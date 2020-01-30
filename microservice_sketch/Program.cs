using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog.Web;
using AspectCore.Extensions.Hosting;

namespace microservice_sketch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog_win.config").GetCurrentClassLogger();
            try
            {
                logger.Info(" start nLog");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex,$" start nLog exception{ ex.Message.ToString()}");
            }
            finally {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceContext()
                .ConfigureDynamicProxy((service,config) => {

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                                                .AddJsonFile("hosts.json")
                                                                .Build();

                    webBuilder.UseConfiguration(configuration);

                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseNLog();
                });
    }
}
