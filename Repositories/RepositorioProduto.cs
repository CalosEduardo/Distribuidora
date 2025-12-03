using Distribuidora.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distribuidora.Repositories
{
    public class RepositorioProdutos : IRepositorioProdutos
    {
        private readonly string _connectionString;

        public RepositorioProdutos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<List<Produto>> ObterTodosAsync()
        {
            var produtos = new List<Produto>();
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT * FROM produtos ORDER BY id", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                produtos.Add(new Produto
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    QuantidadeEstoque = reader.GetInt32(2),
                    PrecoCusto = reader.GetDecimal(3),
                    PrecoVenda = reader.GetDecimal(4),
                    DataCadastro = reader.GetDateTime(5)
                });
            }
            return produtos;
        }

        public async Task<Produto?> ObterPorIdAsync(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT * FROM produtos WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Produto
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    QuantidadeEstoque = reader.GetInt32(2),
                    PrecoCusto = reader.GetDecimal(3),
                    PrecoVenda = reader.GetDecimal(4),
                    DataCadastro = reader.GetDateTime(5)
                };
            }
            return null;
        }

        public async Task AdicionarAsync(Produto produto)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Get next ID (simple auto-increment simulation if not using SERIAL)
            // Ideally DB should handle this, but keeping logic similar to original for now or using MAX+1
            // Better: Use SERIAL in DB. Assuming DB schema exists. 
            // If DB schema is fixed, we might need to manage ID manually or update schema.
            // Let's assume we can let DB handle it or we fetch max ID. 
            // The original code managed IDs manually via config. Let's try to use DB sequence or MAX+1.
            
            using var cmdId = new NpgsqlCommand("SELECT COALESCE(MAX(id), 0) + 1 FROM produtos", conn);
            var result = await cmdId.ExecuteScalarAsync();
            int nextId = result != null ? Convert.ToInt32(result) : 1;
            produto.Id = nextId;

            using var cmd = new NpgsqlCommand(
                "INSERT INTO produtos (id, nome, quantidade_estoque, preco_custo, preco_venda, data_cadastro) " +
                "VALUES (@id, @nome, @qtd, @custo, @venda, @data)", conn);

            cmd.Parameters.AddWithValue("id", produto.Id);
            cmd.Parameters.AddWithValue("nome", produto.Nome);
            cmd.Parameters.AddWithValue("qtd", produto.QuantidadeEstoque);
            cmd.Parameters.AddWithValue("custo", produto.PrecoCusto);
            cmd.Parameters.AddWithValue("venda", produto.PrecoVenda);
            cmd.Parameters.AddWithValue("data", produto.DataCadastro);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task AtualizarAsync(Produto produto)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                "UPDATE produtos SET nome = @nome, quantidade_estoque = @qtd, preco_custo = @custo, preco_venda = @venda " +
                "WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", produto.Id);
            cmd.Parameters.AddWithValue("nome", produto.Nome);
            cmd.Parameters.AddWithValue("qtd", produto.QuantidadeEstoque);
            cmd.Parameters.AddWithValue("custo", produto.PrecoCusto);
            cmd.Parameters.AddWithValue("venda", produto.PrecoVenda);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RemoverAsync(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("DELETE FROM produtos WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Transacao>> ObterTransacoesAsync()
        {
            var transacoes = new List<Transacao>();
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT * FROM transacoes ORDER BY data DESC", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transacoes.Add(new Transacao
                {
                    Id = reader.GetInt32(0),
                    ProdutoId = reader.GetInt32(1),
                    NomeProduto = reader.GetString(2),
                    Tipo = Enum.Parse<TipoTransacao>(reader.GetString(3)),
                    Quantidade = reader.GetInt32(4),
                    ValorUnitario = reader.GetDecimal(5),
                    LucroObtido = reader.GetDecimal(6),
                    Data = reader.GetDateTime(7)
                });
            }
            return transacoes;
        }

        public async Task RegistrarTransacaoAsync(Transacao transacao)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmdId = new NpgsqlCommand("SELECT COALESCE(MAX(id), 0) + 1 FROM transacoes", conn);
            var result = await cmdId.ExecuteScalarAsync();
            int nextId = result != null ? Convert.ToInt32(result) : 1;
            transacao.Id = nextId;

            using var cmd = new NpgsqlCommand(
                "INSERT INTO transacoes (id, produto_id, nome_produto, tipo, quantidade, valor_unitario, lucro_obtido, data) " +
                "VALUES (@id, @prodId, @nome, @tipo, @qtd, @valor, @lucro, @data)", conn);

            cmd.Parameters.AddWithValue("id", transacao.Id);
            cmd.Parameters.AddWithValue("prodId", transacao.ProdutoId);
            cmd.Parameters.AddWithValue("nome", transacao.NomeProduto);
            cmd.Parameters.AddWithValue("tipo", transacao.Tipo.ToString());
            cmd.Parameters.AddWithValue("qtd", transacao.Quantidade);
            cmd.Parameters.AddWithValue("valor", transacao.ValorUnitario);
            cmd.Parameters.AddWithValue("lucro", transacao.LucroObtido);
            cmd.Parameters.AddWithValue("data", transacao.Data);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<decimal> ObterLucroTotalAsync()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT valor FROM config WHERE chave = 'lucro_total'", conn);
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? decimal.Parse(result.ToString()!) : 0;
        }

        public async Task AtualizarLucroTotalAsync(decimal lucro)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                "INSERT INTO config (chave, valor) VALUES ('lucro_total', @valor) " +
                "ON CONFLICT (chave) DO UPDATE SET valor = @valor", conn);
            
            cmd.Parameters.AddWithValue("valor", lucro.ToString());
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
