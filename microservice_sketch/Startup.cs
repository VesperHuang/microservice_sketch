using kooco.common.models;
using microservice_sketch.Middleware;
using microservice_sketch.Models;
using microservice_sketch.Permission;
using microservice_sketch.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Web;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

            #region api_settings set data
            _api_settings = new api_settings();
            Configuration.GetSection("api_settings").Bind(_api_settings);
            #endregion

            #region Encrypt secret words
            //var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            //var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog_win.config").GetCurrentClassLogger();

            // dataBase MySqlConfig = Configuration.GetSection("MySQL").Get<dataBase>();
            // string storage_type = kooco.common.utils.tools.EncryptAES(MySqlConfig.type);
            // string server = kooco.common.utils.tools.EncryptAES(MySqlConfig.server);
            // string port = kooco.common.utils.tools.EncryptAES(MySqlConfig.port);
            // string user_id = kooco.common.utils.tools.EncryptAES(MySqlConfig.user_id);
            // string password = kooco.common.utils.tools.EncryptAES(MySqlConfig.password);
            // string db_name = kooco.common.utils.tools.EncryptAES(MySqlConfig.db_name);

            // logger.Info("MySQL type=" + storage_type);
            // logger.Info("MySQL server=" + server);
            // logger.Info("MySQL port=" + port);
            // logger.Info("MySQL user_id=" + user_id);
            // logger.Info("MySQL password=" + password);
            // logger.Info("MySQL db_name=" + db_name);

            // dataBase RedisConfig = Configuration.GetSection("Redis").Get<dataBase>();
            // string storage_type = kooco.common.utils.tools.EncryptAES(RedisConfig.type,strKey,strIV);
            // string server = kooco.common.utils.tools.EncryptAES(RedisConfig.server,strKey,strIV);
            // string port = kooco.common.utils.tools.EncryptAES(RedisConfig.port,strKey,strIV);
            // string user_id = kooco.common.utils.tools.EncryptAES(RedisConfig.user_id,strKey,strIV);
            // string password = kooco.common.utils.tools.EncryptAES(RedisConfig.password,strKey,strIV);
            // string db_name = kooco.common.utils.tools.EncryptAES(RedisConfig.db_name,strKey,strIV);

            // logger.Info("Redis type=" + storage_type);
            // logger.Info("Redis server=" + server);
            // logger.Info("Redis port=" + port);
            // logger.Info("Redis user_id=" + user_id);
            // logger.Info("Redis password=" + password);
            // logger.Info("Redis db_name=" + db_name);

            // token token  = Configuration.GetSection("token").Get<token>();         
            // string secret = kooco.common.utils.tools.EncryptAES(token.secret,strKey,strIV);
            // string issuer = kooco.common.utils.tools.EncryptAES(token.issuer,strKey,strIV);
            // string audience = kooco.common.utils.tools.EncryptAES(token.audience,strKey,strIV);

            // logger.Info("token secret=" + secret);
            // logger.Info("token issuer=" + issuer);
            // logger.Info("token audience=" + audience);               
            #endregion

            #region use secrets.json to behide secret's data 
            // dataBase MySqlConfig = Configuration.GetSection("MySQL").Get<dataBase>();
            // dataBase RedisConfig = Configuration.GetSection("Redis").Get<dataBase>();
        
            // List<dataBase> storage = new List<dataBase>();
            // storage.Add(MySqlConfig);
            // storage.Add(RedisConfig);
            // _api_settings.storage = storage;

            // token token  = Configuration.GetSection("token").Get<token>();
            // _api_settings.token = token;
            #endregion

            //export api_settings
            api_settings = _api_settings;
        }

        public IConfiguration Configuration { get; }

        public static api_settings api_settings { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region data encrypt decrypt 
            //services.AddDataProtection()
            //    //.PersistKeysToFileSystem(new DirectoryInfo(@"dataProtection")); //save private key
            //    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
            //     {
            //         EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
            //         ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            //     })
            //    .SetApplicationName("MyCommonName");
            ////services.AddSingleton<IApiService, EncryptionService>();

            //var service_proivder = services.BuildServiceProvider();
            //var instance = ActivatorUtilities.CreateInstance<EncryptionService>(service_proivder);
            //var server = instance.Encrypt(_api_settings.storage[0].server);
            #endregion

            //at appsetting.json to setting the function will be used
            #region health check
            if (_api_settings.health_switch) {

                //exception message collection 
                services.AddSingleton<exception_collection>();

                services.AddHealthChecks()
                        .AddServiceHealthCheck();
                services.AddSingleton<IApiService, microservice_sketch.Services.HealthCheckService>();

                //auto health publisher
                if (_api_settings.health_publisher)
                {
                    services.Configure<HealthCheckPublisherOptions>(options =>
                    {
                        options.Delay = TimeSpan.FromSeconds(5);
                        options.Period = TimeSpan.FromSeconds(20);
                    });
                    services.AddSingleton<IHealthCheckPublisher, ReadinessPublisher>();
                }
            }
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
                //ADO Type
                //services.AddScoped<IApiService, DataBase>();

                services.AddDbContext<DBContext>(options =>
                {
                    //here need add function to get different storage type
                    var _connection_string = _api_settings.storage[0].get_connection_string;    
                    
                    options.UseMySql(_connection_string, mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(14, 14), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql);
                    });
                });

                //services.AddSingleton<Iservice_data_repository, service_data_repository>();
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

            #region Swagger
            //https://code-maze.com/swagger-ui-asp-net-core-web-api/
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Microservice Sketch API",
                    Version = "v1",
                    Description = "An API to perform Microservice Sketch operations",
                    TermsOfService = new Uri("http://www.kooco.co/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Vesper Huang",
                        Email = "vesper@kooco.com.tw",
                        Url = new Uri("https://twitter.com/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Microservice Sketch API",
                        Url = new Uri("https://example.com/license"),
                    }
                });

                //add token to swagger header
                c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Example:Bearer eyJhbGciOi...",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Authorization" }
                        },
                        new string[] { }
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DBContext dbContext)
        {

            #region Nlog
            //NLog.LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();      
            NLog.LogManager.LoadConfiguration("nlog_win.config").GetCurrentClassLogger();                  

            NLog.LogManager.Configuration.Variables["connectionString"] = _api_settings.storage[0].get_connection_string;   
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); 
            #endregion


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (_api_settings.health_switch)
            {
                app.UseHealthChecks("/health", 8000, new HealthCheckOptions()
                {
                    ResponseWriter = WriteResponse,
                });
            }

            app.UseSession();

            //middleware aop logic 
            app.UseAOP();
            //if (_api_settings.memory_type.ToLower() == "memory" || _api_settings.memory_type.ToLower() == "redis")
            //{
            //    app.UseAOP();
            //}
            app.UseAuthentication();

            #region Swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            #endregion 

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
                    mobile = "0900000000",
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
