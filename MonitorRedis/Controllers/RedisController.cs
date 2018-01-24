using MonitorRedis.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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

        [ActionName("ObterFilas")]
        public IHttpActionResult Get()
        {
            ConnectionMultiplexer redis = getConnectionRedis();

            foreach (var fila in filas)
            {
                fila.Tamanho = ObterTamanhoDaFila(redis, fila);
            }

            redis.Dispose();

            return Ok(new
            {
                listasDeIntegracoes = filas,
                nivelDeIntensidade = filas.Where(l => l.Tamanho == filas.Max(elem => elem.Tamanho)).FirstOrDefault().Tamanho,
                hostName = System.Environment.MachineName
            });
        }

        private int ObterTamanhoDaFila(ConnectionMultiplexer redis, Fila fila)
        {
            var response = redis.GetDatabase().Execute("LLEN", fila.Nome);
            var responsearray = (RedisValue[])response;
            return Convert.ToInt32(responsearray.Last());
        }

        private ConnectionMultiplexer getConnectionRedis()
        {
            var redisConf = new ConfigurationOptions();
            redisConf.EndPoints.Add("localhost", 6379);
            redisConf.Password = "";

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConf);
            return redis;
        }

        [ActionName("ObterDetalhes")]
        public IHttpActionResult GetDetalhes(int id)
        {
            ConnectionMultiplexer redis = getConnectionRedis();

            var fila = filas.Where(f => f.Id == id).FirstOrDefault();
            fila.Tamanho = ObterTamanhoDaFila(redis, fila);
            var redisValues = redis.GetDatabase().ListRange(fila.Nome, 0, fila.Tamanho);
            redis.Dispose();

            if (redisValues.Length > 0)
                fila.Erros = Array.ConvertAll(redisValues, value => JsonConvert.DeserializeObject(value)).ToList();
            
            return Ok(new
            {
                nome = fila.Nome,
                tamanho = fila.Tamanho,
                erros = fila.Erros,
                hostName = System.Environment.MachineName
            });
        }
    }
}
