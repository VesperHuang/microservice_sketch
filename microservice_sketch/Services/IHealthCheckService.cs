using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using kooco.common.models;
using microservice_sketch.Models.DataTransferObjects;
using microservice_sketch.Models.DataTransferObjects.Response;
using microservice_sketch.Models.DataTransferObjects.Request;
using microservice_sketch.Models.Shared;

using microservice_sketch.Models;
namespace microservice_sketch.Services
{
    public class HealthCheckService: IHealthCheck, IApiService
    {
        private Dictionary<string, List<string>> _exception_collection;

        public HealthCheckService(exception_collection shared_exception_collection)
        {
            this._exception_collection = shared_exception_collection.collection;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            //add logic to read exception count 
            //need define category
            //where to record exception count
            //exception record class field  class_name error_message error_dataTime 

            if (this._exception_collection.Count >= 1)
            {
                //int exception_level_1 = _exception_collection["exception_level_1"].Count;

                #region call slack notify
                api_settings _api_settings = Startup.api_settings;
                if (_api_settings.health_slack_notify)
                {
                    SlackNotifyDTO para = new SlackNotifyDTO();
                    para.Channel = "pinetree";
                    para.Json = (object)"{'message':'test slack notify from health check service'}";

                    Utils.PostJsonMessageToSlackChannel(para);
                }
                #endregion
                return Task.FromResult(HealthCheckResult.Unhealthy("不健康"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Healthy("健康"));
            }
        }

        /// <summary>
        ///     need to define exception level for microservice
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exception_message"></param>
        public void write_exception(string level, string exception_message) {
            if (_exception_collection.ContainsKey(level))
            {
                _exception_collection[level].Add(exception_message);
            }
            else
            {
                List<string> exception_message_collection = new List<string>();
                exception_message_collection.Add(exception_message);

                _exception_collection.Add(level, exception_message_collection);
            }
        }

        public void info(string function_name)
        {
            Console.WriteLine("Health Check Service info " + function_name);
        }

    }
    public static class HealthCheckExtension
    {
        public static IHealthChecksBuilder AddServiceHealthCheck(this IHealthChecksBuilder healthChecksBuilder)
        {
            healthChecksBuilder.AddCheck<HealthCheckService>("HealthCheckService", HealthStatus.Healthy);
            return healthChecksBuilder;
        }
    }

}
