using Distribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distribuidora.Repositories
{
    public interface IRepositorioProdutos
    {
        Task<List<Produto>> ObterTodosAsync();
        Task<Produto?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);
        Task RemoverAsync(int id);
        Task<List<Transacao>> ObterTransacoesAsync();
        Task RegistrarTransacaoAsync(Transacao transacao);
        Task<decimal> ObterLucroTotalAsync();
        Task AtualizarLucroTotalAsync(decimal lucro);
    }
}
