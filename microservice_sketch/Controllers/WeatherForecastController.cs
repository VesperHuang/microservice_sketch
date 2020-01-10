using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using microservice_sketch.Models;
using microservice_sketch.Permission;
namespace microservice_sketch.Controllers
{

    [Authorize(Policy = "Permission")]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController :ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        readonly PermissionRequirement _permissionRequirement;
        DBContext _dbContext;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, PermissionRequirement permissionRequirement, DBContext dbContext)
        {
            _logger = logger;
            _permissionRequirement = permissionRequirement;
            _dbContext = dbContext;
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
                    else {
                        return BadRequest("username or password is error");
                    }
                }
                else {
                    return BadRequest("username or password can't empty");
                }
            }
            catch (Exception ex) {
                Console.WriteLine("login faild error message:{0}", ex.Message.ToString());
                return BadRequest("service exception");
            }
        }

        [AllowAnonymous]
        [HttpGet("/user_info")]
        public IActionResult UserInfo()
        {
            try {
                var result = HttpContext.Items["cache_user_data"];
                if (string.IsNullOrEmpty(result.ToString())) {

                }
                return new JsonResult(result);
            }
            catch(Exception ex) {
                Console.WriteLine("login faild error message:{0}", ex.Message.ToString());
                return BadRequest("service exception");
            }
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            var list = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToList()
            ;


            list.Add(new WeatherForecast { TemperatureC = 10, Summary = User.Identity.Name, Date = DateTime.Now });
            return list.ToArray();
        }

    }
}