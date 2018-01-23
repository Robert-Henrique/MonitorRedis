using MonitorRedis.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MonitorRedis.Controllers
{
    public class RedisController : ApiController
    {
        static List<Fila> filas = new List<Fila>()
        {
            new Fila(1, "portal.integration.queue.dbsii-Error", "dbSII", 0),
            new Fila(2, "portal.integration.queue.dbzim-Error", "dbZIM", 0),
            new Fila(3, "portal.integration.queue.zim-Error", "ZIM", 0)
        };

        public IHttpActionResult Get()
        {
            var redisConf = new ConfigurationOptions();
            redisConf.EndPoints.Add("localhost", 6379);
            redisConf.Password = "";

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConf);
            var dataBase = redis.GetDatabase();

            foreach (var fila in filas)
            {
                var response = redis.GetDatabase().Execute("LLEN", fila.Nome);
                var responsearray = (RedisValue[])response;
                fila.Tamanho = Convert.ToInt32(responsearray.Last());
            }

            redis.Dispose();

            return Ok(new
            {
                listasDeIntegracoes = filas,
                nivelDeIntensidade = filas.Where(l => l.Tamanho == filas.Max(elem => elem.Tamanho)).FirstOrDefault(),
                hostName = System.Environment.MachineName
            });
        }
    }
}
