using kooco.common.models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;

namespace microservice_sketch.Services
{
    public class MemoryCacheService
    {
        private readonly api_settings _api_settings;
        private readonly memory_cache _memory_cache; //api_settings's propetry

        public MemoryCache cache { get; set; }

        /// <summary>
        ///     size_limit: golbal cache size
        ///     instance_size_limit: instance cache size can't  equal or more then golbal cache size
        ///     default:passive
        /// </summary>
        public MemoryCacheService() {
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                                                     .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                                                                     .Build();

            _api_settings = new api_settings();
            configuration.GetSection("api_settings").Bind(_api_settings);

            _memory_cache = _api_settings.memory_cache;

            cache = new MemoryCache(new MemoryCacheOptions()
            {
                //golbal cache max size
                //SizeLimit = (_memory_cache.actions.Exists(x => x == "size_limit") ? _memory_cache.size_limit : 1024)
                SizeLimit = _memory_cache.size_limit
            });

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSize(_memory_cache.instance_size_limit)
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(_memory_cache.expiration));

            if (_memory_cache.initiative)
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                cacheOptions.AddExpirationToken(new CancellationChangeToken(tokenSource.Token));
                tokenSource.CancelAfter(1000 * _memory_cache.expiration);
            }
        }
    }
}
