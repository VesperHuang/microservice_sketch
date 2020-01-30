using kooco.common.models;
using microservice_sketch.Models;
using microservice_sketch.Permission;
using microservice_sketch.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace microservice_sketch.Controllers
{
    [Authorize(Policy = "Permission")]
    [ApiController]
    [Route("[controller]")]
    public class UserCenterController : ControllerBase
    {
        private readonly api_settings _api_settings;
        private readonly PermissionRequirement _permissionRequirement;
        private HealthCheckService _health_check_service;
        private DBContext _dbContext;

        private INlogService _nLog_instance;

        private readonly ILogger<UserCenterController> _logger;

        public UserCenterController(IEnumerable<IApiService> apiServices, PermissionRequirement permissionRequirement, DBContext dbContext,ILogger<UserCenterController> logger) {
            _permissionRequirement = permissionRequirement;
            _dbContext = dbContext;

            _api_settings = Startup.api_settings;
            #region example how to get instance in IEnumerable<IApiService>
            //get service instance
            //IApiService instance = service_data_repository.get_service(apiServices, "AopService");
            //instance.info("get instance from apiServices");

            INlogService nLog_instance = (INlogService)service_data_repository.get_service(apiServices, "NlogService");
            _nLog_instance = nLog_instance;

            _nLog_instance.info("NLog use info test here is values controller comstruct");
            _nLog_instance.fatal("NLog use fatal test here is values controller comstruct");
            #endregion

            _health_check_service = (HealthCheckService)service_data_repository.get_service(apiServices, "HealthCheckService");

            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        public IActionResult Login(string account, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password))
                {
                    var query = (from u in _dbContext.user
                                 join r in _dbContext.user_role on u.user_id equals r.user_id
                                 where u.account == account && u.password == password
                                 select new { r.role_type, u.name, u.account }).FirstOrDefault();

                    if (query != null)
                    {
                        var claims = new Claim[] {
                            new Claim(ClaimTypes.Role, query.role_type),
                            new Claim(ClaimTypes.Name, query.account),
                            new Claim(ClaimTypes.Sid, "kooco"),
                            new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_permissionRequirement.Expiration.TotalSeconds).ToString())
                        };
                        var token = JwtToken.BuildJwtToken(claims, _permissionRequirement);
                        return new JsonResult(token);
                    }
                    else
                    {
                        return BadRequest("username or password is error");
                    }
                }
                else
                {
                    return BadRequest("username or password can't empty");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("login faild error message:{0}", ex.Message.ToString());
                return BadRequest("service exception");
            }
        }

        [AllowAnonymous]
        [HttpGet("/user_info")]
        public IActionResult UserInfo()
        {
            try
            {
                // var loggerFactory = LoggerFactory.Create(builder =>
                // {
                //     builder
                //         //.AddFilter("microservice_sketch.Controllers", LogLevel.Debug)
                //         .AddConsole()
                //         .AddDebug();
                // });
                // ILogger logger = loggerFactory.CreateLogger<UserCenterController>();
                // logger.LogInformation("creater logger ============> log information");            
                // logger.LogDebug("creater logger ============> log debug");     

                _logger.LogTrace("logger ============> Trace");
                _logger.LogCritical("logger ============> Critical");
                _logger.LogDebug("logger ============> Debug");
                _logger.LogWarning("logger ============> Warning");
                _logger.LogInformation("logger ============> Information");
                _logger.LogError("logger ============> Error");                  


                HttpContext.Items.TryGetValue("cache_user_data", out var middleware_value);
                var result = middleware_value?.ToString() ?? "";

                if (result == "")
                {
                    string account = HttpContext.Items["account"].ToString();
                    result = service_data_repository.get_user_data(_dbContext, account);
                }
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                _nLog_instance.fatal("login faild error message:" + ex.Message.ToString());
                return BadRequest("service exception");
            }
        }

        /// <summary>
        ///     make exception to test HealthCheckService
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("/make_exception")]
        public void make_exceptin()
        {
            try
            {
                throw new Exception("from make_exception");
            }
            catch (Exception ex)
            {
                this._health_check_service.write_exception("exception_level_1", ex.Message.ToString());
                //return BadRequest("service exception");
            }
        }
    }
}