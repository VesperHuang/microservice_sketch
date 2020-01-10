using AspectCore.DynamicProxy;
using kooco.common.models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_sketch.Services
{
    public interface IAopService
    {
        void print(string write_something);


        [CacheInterceptorAtrribute]
        string cache_user_data(string account);

        [RedisCacheInterceptorAtrribute]
        string redis_user_data(string account);
    }

    public class AopService : IAopService, IApiService
    {

        private DBContext _dbContext;

        public AopService(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void print(string write_something)
        {
            Console.WriteLine("AOP Service print " + write_something);
        }

        public void info(string function_name)
        {
            Console.WriteLine("AOP Service info " + function_name);
        }

        public string cache_user_data(string account)
        {
            return this.get_user_data(account);
        }

        public string redis_user_data(string account) {
            return this.get_user_data(account);
        }

        private string get_user_data(string account) {
            var result = string.Empty;

            var user = (from u in _dbContext.user
                        join r in _dbContext.user_role on u.user_id equals r.user_id
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
    }

    #region before use dictionary logic
    //public class CacheInterceptorAtrribute : AbstractInterceptorAttribute
    //{
    //    private Dictionary<string, string> cache_dictory = new Dictionary<string, string>();

    //    public override Task Invoke(AspectContext context, AspectDelegate next)
    //    {
    //        var cache_key = string.Join(",", context.Parameters);

    //        if (cache_dictory.ContainsKey(cache_key))
    //        {
    //            context.ReturnValue = cache_dictory[cache_key].ToString();
    //            return Task.CompletedTask;
    //        }

    //        var task = next(context);
    //        var cache_value = context.ReturnValue;
    //        cache_dictory.Add(cache_key, cache_value.ToString());

    //        return task;
    //    }
    //}
    #endregion

    [AttributeUsage(AttributeTargets.Method)]
    public class CacheInterceptorAtrribute : AbstractInterceptorAttribute
    {
        private api_settings _api_settings;
        private readonly memory_cache _memory_cache; //api_settings's propetry
        private MemoryCache _cache;

        public CacheInterceptorAtrribute()
        {

            #region dirty code the same code in the MemoryCasheService.cs
            //here to get cache define
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                                                     .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                                                                     .Build();

            _api_settings = new api_settings();
            configuration.GetSection("api_settings").Bind(_api_settings);
            _memory_cache = _api_settings.memory_cache;
            #endregion

            var instance = Activator.CreateInstance(type: typeof(MemoryCacheService)) as MemoryCacheService;
            _cache = instance.cache;
        }

        public CacheInterceptorAtrribute(MemoryCacheService _memory_cache_service)
        {
            _cache = _memory_cache_service.cache;
        }

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var cache_key = string.Join(",", context.Parameters);

            if (!string.IsNullOrEmpty(_cache.Get<string>(cache_key)))
            {
                context.ReturnValue = _cache.Get<string>(cache_key);
                return Task.CompletedTask;
            }

            var task = next(context);
            var cache_value = context.ReturnValue;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(_memory_cache.instance_size_limit)
                .SetSlidingExpiration(TimeSpan.FromSeconds(_memory_cache.expiration));
            _cache.Set(cache_key, cache_value, cacheEntryOptions);

            return task;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RedisCacheInterceptorAtrribute : AbstractInterceptorAttribute
    {
        RedisService _redis_service;
        public RedisCacheInterceptorAtrribute()
        {
            _redis_service = Activator.CreateInstance(type: typeof(RedisService)) as RedisService;
        }

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var _key = string.Join(",", context.Parameters);

            if (!string.IsNullOrEmpty(_redis_service.redis_read(_key)))
            {
                context.ReturnValue = _redis_service.redis_read(_key);
                return Task.CompletedTask;
            }

            var task = next(context);
            var _value = context.ReturnValue;

            _redis_service.redis_wirte(_key, _value.ToString());
            return task;
        }
    }
}
