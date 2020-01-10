using kooco.common.models;
using microservice_sketch.Models;
using microservice_sketch.Permission;
using microservice_sketch.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace microservice_sketch.Middleware
{
    public class AopMiddleware
    {
        private readonly RequestDelegate _next;

        public AopMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IEnumerable<IApiService> apiServices, Microsoft.Extensions.Options.IOptionsSnapshot<api_settings> options)
        {
            api_settings _api_settings = options.Value;

            if (_api_settings.memory_type.ToLower() == "memory" || _api_settings.memory_type.ToLower() == "redis")
            {
                IApiService instance = service_data_repository.get_service(apiServices, "AopService");
                instance.info("get instance from apiServices");
                IAopService obj = (IAopService)instance;

                obj.print("start aop service");
                //according different path to practice self's aop logic
                switch (context.Request.Path.Value) {
                    #region useful- read response.body
                    //case "/login":
                    //    //check is it cache if hasn's get data from database
                    //    var data = obj.cache_data(context.Request.Query["account"].ToString());
                    //    if (string.IsNullOrEmpty(data))
                    //    {
                    //        string response_content;
                    //        var original_body_stream = context.Response.Body;
                    //        using (var fake_response_body = new MemoryStream())
                    //        {
                    //            context.Response.Body = fake_response_body;
                    //            await _next(context);

                    //            fake_response_body.Seek(0, SeekOrigin.Begin);
                    //            using (var reader = new StreamReader(fake_response_body))
                    //            {
                    //                response_content = await reader.ReadToEndAsync();
                    //                fake_response_body.Seek(0, SeekOrigin.Begin);

                    //                await fake_response_body.CopyToAsync(original_body_stream);
                    //            }
                    //        }
                    //        obj.cache_data(context.Request.Query["account"].ToString(), response_content);
                    //    }
                    //    else
                    //    {
                    //        await context.Response.WriteAsync(data);
                    //    }
                    //    break;
                    #endregion
                    case "/user_info":
                        // aop example 
                        //setp 1: get user account from token
                        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                        var decode_token = await JwtToken.ReadTokenAsync(token);

                        JObject jwtObj = (JObject)JToken.Parse(decode_token);
                        JObject claims_name = (JObject)jwtObj["Payload"][1];
                        string account = ((string)claims_name["Value"]);

                        //setp 2 if cache have data ,return it else get data from database}
                        var cache_user_data = (_api_settings.memory_type.ToLower() == "memory")? obj.cache_user_data(account): obj.redis_user_data(account);

                        //setp 3 response write 
                        //can't return controller response end 
                        //await context.Response.WriteAsync(result);

                        //return to controller
                        context.Items.Add("cache_user_data", cache_user_data);
                        await _next(context);
                        break;

                    default:
                        await _next(context);
                        break;                
                }
                obj.print("end aop service");
            }
            else {
                //aop switch off
                await _next(context);
            }
        }
    }

    public static class middleware_extensions
    {
        public static IApplicationBuilder UseAOP(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AopMiddleware>();
        }
    }
}
