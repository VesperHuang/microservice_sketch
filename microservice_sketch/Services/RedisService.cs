using kooco.common.models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using System;

namespace microservice_sketch.Services
{
    public class RedisService
    {
        private readonly api_settings _api_settings;
        private RedisCache _redis_cache;

        public RedisService()
        {
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                                         .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                                                         .Build();

            _api_settings = new api_settings();
            configuration.GetSection("api_settings").Bind(_api_settings);

            var redis_info = _api_settings.storage[1];

            _redis_cache = new RedisCache(new RedisCacheOptions()
            {
                Configuration = redis_info.server.ToString(),
                InstanceName = redis_info.db_name
            });
        }

        public void redis_wirte(string key, string value) {
            _redis_cache.SetString(key, value, new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(_api_settings.memory_cache.expiration),
            });
        }

        public string redis_read(string key) { 
            return _redis_cache.GetString(key);
        }
    }
}
