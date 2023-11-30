using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Linq;

namespace StackExchangeRedis.API.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly string _redisPassword;

        private ConnectionMultiplexer _multiplexer;
        public IDatabase db { get; set; }

        private readonly ILogger<RedisService> _logger;

        public RedisService(IConfiguration configuration, ILogger<RedisService> logger)
        {
            _redisHost = configuration["Redis:Host"];
            _redisPassword = configuration["Redis:Password"];
            _logger = logger;
        }

        public void RedisConnect()
        {
            try
            {
                var config = ConfigurationOptions.Parse(_redisHost);
                config.Password = _redisPassword;

                _multiplexer = ConnectionMultiplexer.Connect(config);                ;
                _logger.Log(LogLevel.Information, "Redis Sunucusuna Erisildi");

            }
            catch (System.Exception ex)
            {
                _logger.Log(LogLevel.Critical, $"Redis Servera Erisilemedi ! {ex}", ex.Message, null);
            }
        }


        public IDatabase GetDatabase(int db)
        {
            return _multiplexer.GetDatabase(db);
        }
    }
}
