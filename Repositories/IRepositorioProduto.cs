using Distribuidora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribuidora.Repositories
{
    public interface IRepositorioProdutos
    {
        void Salvar(DadosDistribuidora dados);
        DadosDistribuidora Carregar();
    }
}
