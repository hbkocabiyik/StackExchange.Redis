using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchangeRedis.API.Services;
using System.Collections.Generic;
using System;

namespace StackExchangeRedis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly ILogger<RedisController> _logger;
        private readonly IDatabase _database;

        public RedisController(RedisService redisService, ILogger<RedisController> logger)
        {
            _redisService = redisService;
            _logger = logger;
            _database = _redisService.GetDatabase(0);
        }

        [Route("{key}/get")]
        [HttpGet]
        public string Get(string key)
        {
            try
            {
                var result = _database.StringGet(key);

                if (result.HasValue)
                {
                    return result.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex, ex.Message, null);
                throw new Exception(ex.Message);
            }
        }


        [Route("create")]
        [HttpPost]
        public string Post(string key, [FromBody] string value)
        {
            try
            {
                var result = _database.StringSet(key, value);

                if (result)
                {
                    return "Ok";
                }
                else
                {
                    return "Fail";
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex, ex.Message, null);
                throw new Exception(ex.Message);
            }

        }

        [Route("{key}/delete")]
        [HttpDelete]
        public string Delete(string key)
        {
            try
            {
                var result = _database.KeyDelete(key);

                if (result)
                {
                    return "Ok";
                }
                else
                {
                    return "Fail";
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex, ex.Message, null);
                throw new Exception(ex.Message);
            }
        }
    }
}
