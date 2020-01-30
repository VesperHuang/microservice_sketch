using microservice_sketch.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace microservice_sketch.Models
{
    /// <summary>
    /// controls get,set data logic must be here, can't put in controlls
    /// </summary>
    public class service_data_repository : IDisposable
    {
        private bool disposed = false;

        public static IApiService get_service(IEnumerable<IApiService> apiServices, string service_name)
        {
            IApiService result = null;

            foreach (var service in apiServices)
            {
                var key = service.ToString().Split('.')[service.ToString().Split('.').Length - 1];

                if (key == service_name)
                {
                    result = service;
                    break;
                }
            }

            return result;
        }

        public static void service_list(IEnumerable<IApiService> apiServices) {
            IApiService _service;

            foreach (var service in apiServices)
            {
                switch (service)
                {
                    case AopService aop:
                        _service = aop;
                        _service.info("use aop service " + aop.ToString());
                        break;
                    case DataBase database:
                        _service = database;
                        _service.info("use database service " + database.ToString());
                        break;
                    case NlogService nlog:
                        _service = nlog;
                        _service.info("use nlog service " + nlog.ToString());
                        break;
                }
            }
        }

        public static string get_user_data(DBContext dbContext,string account)
        {
            var result = string.Empty;

            var user = (from u in dbContext.user
                        join r in dbContext.user_role on u.user_id equals r.user_id
                        where u.account == account
                        select new
                        {
                            r.role_type,
                            u.account,
                            u.name,
                            u.mobile,
                            u.email,
                            u.ip
                        }).FirstOrDefault();

            result = JsonConvert.SerializeObject(user);
            return result;
        }

        public void info(string function_name)
        {
            Console.WriteLine("service_data_repository info " + function_name);
        }

        #region dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {

                }
                disposed = true;
            }
        }

        ~service_data_repository()
        {
            Dispose(false);
        }
        #endregion
    }
}
