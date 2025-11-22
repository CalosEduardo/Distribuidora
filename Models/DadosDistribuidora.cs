using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Distribuidora.Models
{
    public class DadosDistribuidora
    {
        public List<Produto> Produtos { get; set; } = new List<Produto>();
        public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
        public decimal LucroTotal { get; set; }
        public int ProximoIdProduto { get; set; } = 1;
        public int ProximoIdTransacao { get; set; } = 1;
    }
}
