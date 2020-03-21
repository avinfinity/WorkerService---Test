using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _httpClient = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _httpClient?.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                var response = await _httpClient.GetAsync(_configuration["CheckURL"], stoppingToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Website {_configuration["CheckURL"]} is up and running. Checked at {DateTimeOffset.Now}");
                }
                else
                {
                    _logger.LogInformation($"Website {_configuration["CheckURL"]} is down. Checked at {DateTimeOffset.Now} " +
                        $"{ Environment.NewLine} Status Code:{response.StatusCode}");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}