using Distribuidora.Models;
using Distribuidora.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distribuidora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly IRepositorioProdutos _repositorio;

        public TransacoesController(IRepositorioProdutos repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Transacao>>> Get()
        {
            return await _repositorio.ObterTransacoesAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Transacao>> Post(Transacao transacao)
        {
            // Logic to update stock and profit should ideally be in a Service, 
            // but for simplicity we can put it here or rely on the client to send correct data 
            // (though server side validation is better).
            // For this migration, I'll assume the client sends the transaction details 
            // and we might need to update the product stock here as well if not done by client.
            // However, the original logic updated stock AND created transaction.
            
            // Let's implement the stock update logic here to ensure consistency.
            
            var produto = await _repositorio.ObterPorIdAsync(transacao.ProdutoId);
            if (produto == null)
                return BadRequest("Produto não encontrado");

            // Set transaction date if not provided
            if (transacao.Data == default(DateTime))
                transacao.Data = DateTime.UtcNow;

            if (transacao.Tipo == TipoTransacao.Entrada)
            {
                produto.QuantidadeEstoque += transacao.Quantidade;
                transacao.ValorUnitario = produto.PrecoCusto;
                transacao.LucroObtido = 0; // No profit on stock entry
            }
            else if (transacao.Tipo == TipoTransacao.Saida)
            {
                if (produto.QuantidadeEstoque < transacao.Quantidade)
                    return BadRequest("Estoque insuficiente");
                
                produto.QuantidadeEstoque -= transacao.Quantidade;
                
                // Calculate profit
                decimal lucro = (produto.PrecoVenda - produto.PrecoCusto) * transacao.Quantidade;
                transacao.LucroObtido = lucro;
                transacao.ValorUnitario = produto.PrecoVenda;

                // Update total profit
                var currentProfit = await _repositorio.ObterLucroTotalAsync();
                await _repositorio.AtualizarLucroTotalAsync(currentProfit + lucro);
            }

            await _repositorio.AtualizarAsync(produto);
            await _repositorio.RegistrarTransacaoAsync(transacao);

            return CreatedAtAction(nameof(Get), new { id = transacao.Id }, transacao);
        }
    }
}
