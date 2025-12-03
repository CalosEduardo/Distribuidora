using Distribuidora.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace Distribuidora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IRepositorioProdutos _repositorio;

        public DashboardController(IRepositorioProdutos repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetStats()
        {
            var produtos = await _repositorio.ObterTodosAsync();
            var transacoes = await _repositorio.ObterTransacoesAsync();
            var lucroTotal = await _repositorio.ObterLucroTotalAsync();

            var totalEstoque = produtos.Sum(p => p.QuantidadeEstoque);
            var valorEstoque = produtos.Sum(p => p.PrecoCusto * p.QuantidadeEstoque);
            var estoqueBaixo = produtos.Where(p => p.QuantidadeEstoque <= 5).Count();

            return new
            {
                TotalProdutos = produtos.Count,
                TotalTransacoes = transacoes.Count,
                LucroTotal = lucroTotal,
                TotalEstoque = totalEstoque,
                ValorEstoque = valorEstoque,
                AlertasEstoque = estoqueBaixo
            };
        }
    }
}
