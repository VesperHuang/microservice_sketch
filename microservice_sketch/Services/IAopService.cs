using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_sketch.Services
{
    public interface IAopService
    {
        void print(string write_something);
    }

    public class AopService : IAopService,IApiService {
        public void print(string write_something) {
            Console.WriteLine("AOP Service print " + write_something);
        }

        public void info(string function_name)
        {
            Console.WriteLine("AOP Service info " + function_name);
        }
    }
}
