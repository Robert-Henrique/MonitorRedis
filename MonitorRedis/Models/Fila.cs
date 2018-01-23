using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitorRedis.Models
{
    public class Fila
    {
        public string Nome { get; set; }
        public string NomeAbreviado { get; set; }
        public int Tamanho { get; set; }

        public Fila(string nome, string nomeAbreviado, int tamanho)
        {
            this.Nome = nome;
            this.NomeAbreviado = nomeAbreviado;
            this.Tamanho = tamanho;
        }
    }
}