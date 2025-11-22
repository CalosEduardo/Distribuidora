using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribuidora.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public TipoTransacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal LucroObtido { get; set; }
        public DateTime Data { get; set; }

        public Transacao() { }

        public Transacao(int id, int produtoId, string nomeProduto, TipoTransacao tipo,
                            int quantidade, decimal valorUnitario, decimal lucro)
            {
                Id = id;
                ProdutoId = produtoId;
                NomeProduto = nomeProduto;
                Tipo = tipo;
                Quantidade = quantidade;
                ValorUnitario = valorUnitario;
                LucroObtido = lucro;
                Data = DateTime.Now;
            }

            public override string ToString()
            {
                string tipoStr = Tipo == TipoTransacao.Entrada ? "ENTRADA" : "VENDA";
                return $"[{Data:dd/MM/yyyy HH:mm}] {tipoStr} | {NomeProduto} | Qtd: {Quantidade} | " +
                       $"Valor: {ValorUnitario:C} | Lucro: {LucroObtido:C}";
            }
        }
}
