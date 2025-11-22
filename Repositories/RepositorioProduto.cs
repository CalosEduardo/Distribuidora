using Distribuidora.Config;
using Distribuidora.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribuidora.Repositories
{
    public class RepositorioProdutos : IRepositorioProdutos
    {
        private readonly string connectionString;

        public RepositorioProdutos()
        {
            connectionString = DatabaseConfig.ConnectionString;
        }

        public void Salvar(DadosDistribuidora dados)
        {
            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                using var transaction = conn.BeginTransaction();

                try
                {
                    // Salvar produtos
                    SalvarProdutos(conn, dados.Produtos);

                    // Salvar transações
                    SalvarTransacoes(conn, dados.Transacoes);

                    // Salvar configurações
                    SalvarConfig(conn, "lucro_total", dados.LucroTotal.ToString());
                    SalvarConfig(conn, "proximo_id_produto", dados.ProximoIdProduto.ToString());
                    SalvarConfig(conn, "proximo_id_transacao", dados.ProximoIdTransacao.ToString());

                    transaction.Commit();
                    Console.WriteLine("Dados salvos com sucesso no Neon Database.");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar dados: {ex.Message}");
            }
        }

        public DadosDistribuidora Carregar()
        {
            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var dados = new DadosDistribuidora
                {
                    Produtos = CarregarProdutos(conn),
                    Transacoes = CarregarTransacoes(conn),
                    LucroTotal = decimal.Parse(CarregarConfig(conn, "lucro_total") ?? "0"),
                    ProximoIdProduto = int.Parse(CarregarConfig(conn, "proximo_id_produto") ?? "1"),
                    ProximoIdTransacao = int.Parse(CarregarConfig(conn, "proximo_id_transacao") ?? "1")
                };

                Console.WriteLine($"Dados carregados: {dados.Produtos.Count} produtos, {dados.Transacoes.Count} transações.");
                return dados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar dados: {ex.Message}");
                return new DadosDistribuidora();
            }
        }

        private void SalvarProdutos(NpgsqlConnection conn, List<Produto> produtos)
        {
            // Limpar tabela
            using (var cmd = new NpgsqlCommand("DELETE FROM produtos", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // Inserir produtos
            foreach (var p in produtos)
            {
                using var cmd = new NpgsqlCommand(
                    "INSERT INTO produtos (id, nome, quantidade_estoque, preco_custo, preco_venda, data_cadastro) " +
                    "VALUES (@id, @nome, @qtd, @custo, @venda, @data)", conn);

                cmd.Parameters.AddWithValue("id", p.Id);
                cmd.Parameters.AddWithValue("nome", p.Nome);
                cmd.Parameters.AddWithValue("qtd", p.QuantidadeEstoque);
                cmd.Parameters.AddWithValue("custo", p.PrecoCusto);
                cmd.Parameters.AddWithValue("venda", p.PrecoVenda);
                cmd.Parameters.AddWithValue("data", p.DataCadastro);

                cmd.ExecuteNonQuery();
            }
        }

        private void SalvarTransacoes(NpgsqlConnection conn, List<Transacao> transacoes)
        {
            // Limpar tabela
            using (var cmd = new NpgsqlCommand("DELETE FROM transacoes", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // Inserir transações
            foreach (var t in transacoes)
            {
                using var cmd = new NpgsqlCommand(
                    "INSERT INTO transacoes (id, produto_id, nome_produto, tipo, quantidade, valor_unitario, lucro_obtido, data) " +
                    "VALUES (@id, @prodId, @nome, @tipo, @qtd, @valor, @lucro, @data)", conn);

                cmd.Parameters.AddWithValue("id", t.Id);
                cmd.Parameters.AddWithValue("prodId", t.ProdutoId);
                cmd.Parameters.AddWithValue("nome", t.NomeProduto);
                cmd.Parameters.AddWithValue("tipo", t.Tipo.ToString());
                cmd.Parameters.AddWithValue("qtd", t.Quantidade);
                cmd.Parameters.AddWithValue("valor", t.ValorUnitario);
                cmd.Parameters.AddWithValue("lucro", t.LucroObtido);
                cmd.Parameters.AddWithValue("data", t.Data);

                cmd.ExecuteNonQuery();
            }
        }

        private void SalvarConfig(NpgsqlConnection conn, string chave, string valor)
        {
            using var cmd = new NpgsqlCommand(
                "INSERT INTO config (chave, valor) VALUES (@chave, @valor) " +
                "ON CONFLICT (chave) DO UPDATE SET valor = @valor", conn);

            cmd.Parameters.AddWithValue("chave", chave);
            cmd.Parameters.AddWithValue("valor", valor);

            cmd.ExecuteNonQuery();
        }

        private List<Produto> CarregarProdutos(NpgsqlConnection conn)
        {
            var produtos = new List<Produto>();

            using var cmd = new NpgsqlCommand("SELECT * FROM produtos ORDER BY id", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
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

        private List<Transacao> CarregarTransacoes(NpgsqlConnection conn)
        {
            var transacoes = new List<Transacao>();

            using var cmd = new NpgsqlCommand("SELECT * FROM transacoes ORDER BY data DESC", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
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

        private string CarregarConfig(NpgsqlConnection conn, string chave)
        {
            using var cmd = new NpgsqlCommand("SELECT valor FROM config WHERE chave = @chave", conn);
            cmd.Parameters.AddWithValue("chave", chave);

            var result = cmd.ExecuteScalar();
            return result?.ToString();
        }
    }
}
