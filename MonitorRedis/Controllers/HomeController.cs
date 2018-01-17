using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MonitorRedis.Controllers
{
    public class HomeController : ApiController
    {
        static List<string> keys = new List<string>() { "portal.integration.queue",
                                                        "portal.integration.queue.dbsii",
                                                        "portal.integration.queue.dbzim",
                                                        "portal.integration.queue.zim" };
        public IHttpActionResult Get()
        {
            var redisConf = new ConfigurationOptions();
            redisConf.EndPoints.Add("localhost", 6379);
            redisConf.Password = "";

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConf);
            var dataBase = redis.GetDatabase();

            Dictionary<string, int> listas = new Dictionary<string, int>();

            foreach (var key in keys)
            {
                var response = redis.GetDatabase().Execute("LLEN", key);
                var responsearray = (RedisValue[])response;
                listas.Add(key, Convert.ToInt32(responsearray.Last()));
            }

            return Ok(new
            {
                listasDeIntegracoes = listas.Select(x => new
                {
                    nome = x.Key,
                    tamanho = x.Value
                }),
                nivelDeIntensidade = listas.Where(l => l.Value == listas.Max(elem => elem.Value)).FirstOrDefault().Value,
                hostName = System.Environment.MachineName
            });
        }
    }
}
