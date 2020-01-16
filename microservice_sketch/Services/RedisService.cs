using kooco.common.models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using System;

using Microsoft.AspNetCore.DataProtection;

namespace microservice_sketch.Services
{
    public class RedisService
    {
        private readonly api_settings _api_settings;
        private RedisCache _redis_cache;

        public RedisService()
        {
            _api_settings = Startup.api_settings;
            var redis_info = _api_settings.storage[1];

            #region No DI type encrypt example
            //var dataProtectionProvider = DataProtectionProvider.Create("redis");
            //var protector = dataProtectionProvider.CreateProtector("redis");

            //var type = redis_info.server.ToString();
            //var type_encrypt = protector.Protect(type);

            //var server = redis_info.server.ToString();
            //var server_encrypt = protector.Protect(server);

            //var port = redis_info.server.ToString();
            //var port_encrypt = protector.Protect(port);

            //var db_name = redis_info.server.ToString();
            //var db_name_encrypt = protector.Protect(db_name);            
            #endregion

            _redis_cache = new RedisCache(new RedisCacheOptions()
            {
                Configuration = redis_info.server,
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
