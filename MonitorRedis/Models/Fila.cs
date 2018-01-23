using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitorRedis.Models
{
    public class Fila
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string NomeAbreviado { get; set; }
        public int Tamanho { get; set; }

        public Fila(int id, string nome, string nomeAbreviado, int tamanho)
        {
            this.Id = id;
            this.Nome = nome;
            this.NomeAbreviado = nomeAbreviado;
            this.Tamanho = tamanho;
        }
    }
}