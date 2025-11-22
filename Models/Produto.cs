using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribuidora.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int QuantidadeEstoque { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public DateTime DataCadastro { get; set; }

        public Produto() { }

        public Produto(int id, string nome, int quantidade, decimal precoCusto, decimal precoVenda)
        {
            Id = id;
            Nome = nome;
            QuantidadeEstoque = quantidade;
            PrecoCusto = precoCusto;
            PrecoVenda = precoVenda;
            DataCadastro = DateTime.Now;
        }

        public decimal MargemLucro => PrecoVenda > 0 ? ((PrecoVenda - PrecoCusto) / PrecoVenda) * 100 : 0;

        public override string ToString()
        {
            return $"ID: {Id} | {Nome} | Estoque: {QuantidadeEstoque} | " +
                   $"Custo: {PrecoCusto:C} | Venda: {PrecoVenda:C} | Margem: {MargemLucro:F1}%";
        }
    }
}

