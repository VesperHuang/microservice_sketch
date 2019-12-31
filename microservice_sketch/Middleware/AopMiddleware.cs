using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using kooco.common.models;
using microservice_sketch.Services;

namespace microservice_sketch.Middleware
{
    public class AopMiddleware
    {
        private readonly RequestDelegate _next;

        public AopMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApiService aop_service, Microsoft.Extensions.Options.IOptionsSnapshot<api_settings> options)
        {
            api_settings _api_settings = options.Value;

            if (_api_settings.memory_switch){
                //走 AOP 邏輯
                //檢查是否有緩存 有 return 緩存的資料，沒有 從資料庫取資料(業務邏輯)

                //context.ReturnValue = 緩存資料 OR 資料庫資料;

                aop_service.info("start aop service");
                await _next(context);
                aop_service.info("end aop service");
            }
            else {
                //沒有緩存 走業務邏輯
                await _next(context);
            }
        }
    }

    public static class MyMWExtensions
    {
        public static IApplicationBuilder UseAOP(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AopMiddleware>();
        }
    }
}
