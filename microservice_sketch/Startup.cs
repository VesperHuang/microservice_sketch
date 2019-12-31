using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


using kooco.common.models;
using microservice_sketch.Services;
using microservice_sketch.Middleware;
using Polly;

namespace microservice_sketch
{
    public class Startup
    {
        private readonly api_settings _api_settings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //_api_settings = Configuration.Get<api_settings>();

            _api_settings = new api_settings();
            Configuration.GetSection("api_settings").Bind(_api_settings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        
        {
            //在 appsetting.json 的 api_settings 裡需設定 該功能 服務中才能使用
            if (_api_settings.memory_switch) {
                services.AddScoped<IApiService, AopService>();
            }

            if (_api_settings.dataBase_switch) {
                services.AddScoped<IApiService, DataBase>();
            }

            //injection httpClient for ipstack
            services.AddHttpClient("ipstack", c =>
            {
                c.BaseAddress = new Uri("http://api.ipstack.com/");
            })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(3)))
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(2, TimeSpan.FromSeconds(10)));
                

            services.Configure<api_settings>(Configuration.GetSection("api_settings"));           
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (_api_settings.memory_switch)
            {
                app.UseAOP();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
