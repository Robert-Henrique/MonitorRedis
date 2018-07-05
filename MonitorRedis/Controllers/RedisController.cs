using MonitorRedis.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace MonitorRedis.Controllers
{
    public class RedisController : ApiController
    {
        static List<Fila> filasIntegracao = new List<Fila>()
        {
            new Fila(1, "portal.integration.queue.dbsii", "dbSII - Integração", 0),
            new Fila(2, "portal.integration.queue.dbzim", "dbZIM - Integração", 0),
            new Fila(3, "portal.integration.queue.zim", "ZIM - Integração", 0),
            new Fila(4, "portal.integration.queue.mysql", "MySql - Integração", 0)
        };

        static List<Fila> filasIntegracaoComErro = new List<Fila>()
        {
            new Fila(1, "portal.integration.queue.dbsii-Error", "dbSII - Erro", 0),
            new Fila(2, "portal.integration.queue.dbzim-Error", "dbZIM - Erro", 0),
            new Fila(3, "portal.integration.queue.zim-Error", "ZIM - Erro", 0),
            new Fila(4, "portal.integration.queue.mysql-Error", "MySql - Erro", 0)
        };

        [ActionName("ObterFilas")]
        public IHttpActionResult Get()
        {
            ConnectionMultiplexer redis = GetConnectionRedis();

            foreach (var fila in filasIntegracao)
                fila.Tamanho = ObterTamanhoDaFila(redis, fila);

            foreach (var fila in filasIntegracaoComErro)
                fila.Tamanho = ObterTamanhoDaFila(redis, fila);

            var informacoesServidor = ObterInformacaoServidor(redis);

            redis.Close();
            redis.Dispose();

            string hostName = Dns.GetHostName();
            return Ok(new
            {
                filasDeIntegracoes = filasIntegracao,
                filasDeIntegracoesComErros = filasIntegracaoComErro,
                nivelDeIntensidade = filasIntegracaoComErro.Where(l => l.Tamanho == filasIntegracaoComErro.Max(elem => elem.Tamanho)).FirstOrDefault().Tamanho,
                hostName,
                IP = Dns.GetHostByName(hostName).AddressList[0].ToString(),
                InformacoesServidor = informacoesServidor
            });
        }

        private int ObterTamanhoDaFila(ConnectionMultiplexer redis, Fila fila)
        {
            var response = redis.GetDatabase().Execute("LLEN", fila.Nome);
            var responsearray = (RedisValue[])response;
            return Convert.ToInt32(responsearray.Last());
        }

        private List<InformacaoServidor> ObterInformacaoServidor(ConnectionMultiplexer redis)
        {
            var informacoesServidor = new List<InformacaoServidor>();

            var infoRedis = redis.GetDatabase().Execute("INFO").ToString();

            string[] stringSeparators = new string[] { "#" };
            var result = infoRedis.Split(stringSeparators, StringSplitOptions.None);

            foreach (var item in result.ToList())
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                string titulo = item.Trim().Substring(0, item.Trim().IndexOf("\r\n"));
                informacoesServidor.Add(new InformacaoServidor() { Titulo = titulo, Descricao = item.Trim() });
            }

            return informacoesServidor;
        }

        private ConnectionMultiplexer GetConnectionRedis()
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
            ConnectionMultiplexer redis = GetConnectionRedis();

            var fila = filasIntegracaoComErro.Where(f => f.Id == id).FirstOrDefault();
            fila.Tamanho = ObterTamanhoDaFila(redis, fila);
            var redisValues = redis.GetDatabase().ListRange(fila.Nome, 0, fila.Tamanho);

            redis.Close();
            redis.Dispose();

            fila.Erros = new List<object>();

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

        [HttpDelete]
        public IHttpActionResult Delete(int filaId, string errorTimeStamp)
        {
            ConnectionMultiplexer redis = GetConnectionRedis();

            var fila = filasIntegracaoComErro.Where(f => f.Id == filaId).FirstOrDefault();
            fila.Tamanho = ObterTamanhoDaFila(redis, fila);
            var database = redis.GetDatabase();
            var redisValues = database.ListRange(fila.Nome, 0, fila.Tamanho);

            if (redisValues.Length > 0)
            {
                for (int i = 0; i < redisValues.Length; i++)
                {
                    var redisValue = redisValues[i];
                    bool removeItem = redisValue.ToString().Contains(errorTimeStamp);

                    if (removeItem)
                    {
                        database.ListRemove(fila.Nome, redisValue);
                        break;
                    }
                }
            }

            redis.Close();
            redis.Dispose();

            return Ok();
        }
    }
}
