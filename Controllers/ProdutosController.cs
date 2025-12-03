using Distribuidora.Models;
using Distribuidora.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distribuidora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IRepositorioProdutos _repositorio;

        public ProdutosController(IRepositorioProdutos repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Produto>>> Get()
        {
            return await _repositorio.ObterTodosAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> Get(int id)
        {
            var produto = await _repositorio.ObterPorIdAsync(id);
            if (produto == null)
                return NotFound();
            return produto;
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> Post(Produto produto)
        {
            await _repositorio.AdicionarAsync(produto);
            return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Produto produto)
        {
            if (id != produto.Id)
                return BadRequest();

            var existing = await _repositorio.ObterPorIdAsync(id);
            if (existing == null)
                return NotFound();

            await _repositorio.AtualizarAsync(produto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repositorio.ObterPorIdAsync(id);
            if (existing == null)
                return NotFound();

            await _repositorio.RemoverAsync(id);
            return NoContent();
        }
    }
}
