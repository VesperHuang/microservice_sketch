using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Web;

namespace microservice_sketch.Services
{
    public interface INlogService
    {
        void info(string write_something);

        void fatal(string write_something);
    }

    public class NlogService : INlogService, IApiService{

        public void info(string write_something) {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            logger.Info(write_something);
        }

        public void fatal(string write_something) {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            logger.Fatal(write_something);
        }
    }

}
