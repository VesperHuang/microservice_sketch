using kooco.common.models;
using microservice_sketch.Middleware;
using microservice_sketch.Models;
using microservice_sketch.Permission;
using microservice_sketch.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace microservice_sketch
{
    public class Startup
    {
        private readonly api_settings _api_settings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _api_settings = new api_settings();
            Configuration.GetSection("api_settings").Bind(_api_settings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region health check
            //exception message collection 
            services.AddSingleton<exception_collection>();

            services.AddHealthChecks()
                    .AddServiceHealthCheck();
            services.AddSingleton<IApiService, microservice_sketch.Services.HealthCheckService>();
            #endregion

            #region token role
            var signing_key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_api_settings.token.secret));
            var signing_credentials = new SigningCredentials(signing_key, SecurityAlgorithms.HmacSha256);

            //here must be a role table define role vist path
            var userPerssions = new List<UserPermission> {
                     new UserPermission{  Url="/weatherforecast",Name="admin"},
                     new UserPermission{  Url="/user_info",Name="admin"}

                };
            var permissionRequirement = new PermissionRequirement("/denied", userPerssions, ClaimTypes.Role, _api_settings.token.issuer, _api_settings.token.audience, signing_credentials, TimeSpan.FromSeconds(1000));

            services.AddAuthorization(opt =>
            {

                opt.AddPolicy("Permission", policy =>
                {
                    policy.Requirements.Add(permissionRequirement);
                });
            })
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signing_key,
                        ValidateIssuer = true,
                        ValidIssuer = _api_settings.token.issuer,
                        ValidateAudience = true,
                        ValidAudience = _api_settings.token.audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true
                    };
                });

            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);
            #endregion

            //at appsetting.json to setting the function will be used
            if (_api_settings.memory_type.ToLower() == "memory") {
                services.AddSingleton<MemoryCacheService>();
                services.AddScoped<IApiService, AopService>();
            }

            if (_api_settings.memory_type.ToLower() == "redis")
            {
                services.AddSingleton<RedisService>();
                services.AddScoped<IApiService, AopService>();
            }

            if (_api_settings.dataBase_switch) {
                services.AddScoped<IApiService, DataBase>();

                services.AddDbContext<DBContext>(options =>
                {

                    //here need add function to get different storage type
                    var _connection_string = _api_settings.storage[0].get_connection_string;    
                    
                    options.UseMySql(_connection_string, mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(14, 14), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql);
                    });
                });
            }

            if (_api_settings.nlogger_switch)
            {
                services.AddScoped<IApiService, NlogService>();
            }

            //injection httpClient for ipstack
            services.AddHttpClient("ipstack", c =>
            {
                c.BaseAddress = new Uri("http://api.ipstack.com/"); 
            })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(3)))
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(2, TimeSpan.FromSeconds(10)));
                
            services.Configure<api_settings>(Configuration.GetSection("api_settings"));
            
            services.AddControllers(opt =>
            {
                opt.EnableEndpointRouting = false;
            });

            services.AddMvc()
                .AddSessionStateTempDataProvider();

            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DBContext dbContext)
        {
            app.UseHealthChecks("/health", 8000, new HealthCheckOptions()
            {
                ResponseWriter = WriteResponse,
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            //middleware aop logic 
            app.UseAOP();
            //if (_api_settings.memory_type.ToLower() == "memory" || _api_settings.memory_type.ToLower() == "redis")
            //{
            //    app.UseAOP();
            //}

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMvcWithDefaultRoute();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            InsertData(dbContext);
        }

        private static void InsertData(DBContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            int count = (from u in dbContext.user select u.user_id).Count();
            if (count == 0) {
                var user = new user
                {
                    name = "kooco admin",
                    account = "kooco_admin",
                    password = "abc12345",
                    mobile = "0937000000",
                    email = "admin@kooco.com.tw",
                    ip = "10.10.1.65",
                };
                dbContext.user.Add(user);

                var user_role = new user_role
                {
                    user_id = 1,
                    role_type = "admin"
                };
                dbContext.user_role.Add(user_role);

                dbContext.SaveChanges();
            }
        }

        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }
}
