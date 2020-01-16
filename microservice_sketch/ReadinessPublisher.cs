using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;


namespace microservice_sketch
{
    public class ReadinessPublisher: IHealthCheckPublisher
    {
        private readonly ILogger _logger;

        public ReadinessPublisher(ILogger<ReadinessPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"{ DateTime.UtcNow} Readiness Publisher : {report.Status},{report.TotalDuration},{report.Entries.Count}");

            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }
    }
}
