using Distribuidora.Models;
using Distribuidora.Repositories;
using Distribuidora.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribuidora.Services
{
    public class DistribuidoraService
    {

        private Dictionary<int, Produto> produtos = new Dictionary<int, Produto>();
        private List<Transacao> transacoes = new List<Transacao>();
        private decimal lucroTotal = 0;
        private int proximoIdProduto = 1;
        private int proximoIdTransacao = 1;
        private readonly IRepositorioProdutos repositorio;

        public DistribuidoraService(IRepositorioProdutos repositorio)
        {
            this.repositorio = repositorio;
        }

        public void CarregarDados()
        {
            var dados = repositorio.Carregar();

            produtos.Clear();
            foreach (var p in dados.Produtos)
            {
                produtos[p.Id] = p;
            }

            transacoes = dados.Transacoes;
            lucroTotal = dados.LucroTotal;
            proximoIdProduto = dados.ProximoIdProduto;
            proximoIdTransacao = dados.ProximoIdTransacao;

            System.Threading.Thread.Sleep(1000);
        }

        public void SalvarDados()
        {
            var dados = new DadosDistribuidora
            {
                Produtos = produtos.Values.ToList(),
                Transacoes = transacoes,
                LucroTotal = lucroTotal,
                ProximoIdProduto = proximoIdProduto,
                ProximoIdTransacao = proximoIdTransacao
            };

            repositorio.Salvar(dados);
        }

        public void CadastrarProduto()
        {
            Console.WriteLine("=== CADASTRAR NOVO PRODUTO ===\n");

            string nome = InputValidator.LerTextoObrigatorio("Nome do produto");

            // Verifica duplicidade
            if (produtos.Values.Any(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
            {
                ConsoleHelper.ExibirAviso("Já existe um produto com este nome!");
                if (!InputValidator.ConfirmarAcao("Deseja continuar mesmo assim?"))
                    return;
            }

            int quantidade = InputValidator.LerInteiroPositivo("Quantidade inicial");
            decimal precoCusto = InputValidator.LerDecimalPositivo("Preço de custo");
            decimal precoVenda = InputValidator.LerDecimalMaiorQue("Preço de venda", precoCusto);

            var produto = new Produto(proximoIdProduto++, nome, quantidade, precoCusto, precoVenda);
            produtos[produto.Id] = produto;

            ConsoleHelper.ExibirSucesso($"Produto cadastrado com sucesso! ID: {produto.Id}");
        }

        public void EntradaEstoque()
        {
            Console.WriteLine("=== ENTRADA DE ESTOQUE ===\n");

            if (!ListarProdutos())
                return;

            int id = InputValidator.LerInteiroPositivo("\nDigite o ID do produto");

            if (!produtos.TryGetValue(id, out var produto))
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            Console.WriteLine($"\nProduto selecionado: {produto.Nome}");
            Console.WriteLine($"Estoque atual: {produto.QuantidadeEstoque}");

            int quantidade = InputValidator.LerInteiroPositivo("Quantidade a adicionar");

            int estoqueAnterior = produto.QuantidadeEstoque;
            produto.QuantidadeEstoque += quantidade;

            // Registrar transação
            var transacao = new Transacao(proximoIdTransacao++, produto.Id, produto.Nome,
                                         TipoTransacao.Entrada, quantidade, produto.PrecoCusto, 0);
            transacoes.Add(transacao);

            ConsoleHelper.ExibirSucesso("Estoque atualizado!");
            Console.WriteLine($"  Anterior: {estoqueAnterior} → Novo: {produto.QuantidadeEstoque}");
        }

        public void SaidaEstoque()
        {
            Console.WriteLine("=== SAÍDA DE ESTOQUE (VENDA) ===\n");

            if (!ListarProdutos())
                return;

            int id = InputValidator.LerInteiroPositivo("\nDigite o ID do produto");

            if (!produtos.TryGetValue(id, out var produto))
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            Console.WriteLine($"\nProduto selecionado: {produto.Nome}");
            Console.WriteLine($"Estoque disponível: {produto.QuantidadeEstoque}");
            Console.WriteLine($"Preço de venda: {produto.PrecoVenda:C}");

            int quantidade = InputValidator.LerInteiroPositivo("Quantidade a vender");

            if (quantidade > produto.QuantidadeEstoque)
            {
                ConsoleHelper.ExibirErro("Estoque insuficiente!");
                return;
            }

            // Confirmar venda
            decimal valorTotal = produto.PrecoVenda * quantidade;
            decimal lucro = (produto.PrecoVenda - produto.PrecoCusto) * quantidade;

            Console.WriteLine($"\n--- RESUMO DA VENDA ---");
            Console.WriteLine($"Quantidade: {quantidade}");
            Console.WriteLine($"Valor total: {valorTotal:C}");
            Console.WriteLine($"Lucro estimado: {lucro:C}");

            if (!InputValidator.ConfirmarAcao("\nConfirmar venda?"))
            {
                Console.WriteLine("Venda cancelada.");
                return;
            }

            produto.QuantidadeEstoque -= quantidade;
            lucroTotal += lucro;

            // Registrar transação
            var transacao = new Transacao(proximoIdTransacao++, produto.Id, produto.Nome,
                                         TipoTransacao.Saida, quantidade, produto.PrecoVenda, lucro);
            transacoes.Add(transacao);

            ConsoleHelper.ExibirSucesso("Venda realizada com sucesso!");
            Console.WriteLine($"  Lucro obtido: {lucro:C}");
            Console.WriteLine($"  Estoque restante: {produto.QuantidadeEstoque}");

            // Alerta de estoque baixo
            if (produto.QuantidadeEstoque <= 5)
            {
                ConsoleHelper.ExibirAviso($"ALERTA: Estoque baixo para '{produto.Nome}'!");
            }
        }

        public bool ListarProdutos()
        {
            Console.WriteLine("=== PRODUTOS EM ESTOQUE ===\n");

            if (produtos.Count == 0)
            {
                Console.WriteLine("Nenhum produto cadastrado.");
                return false;
            }

            foreach (var p in produtos.Values.OrderBy(x => x.Id))
            {
                Console.WriteLine(p);
            }

            Console.WriteLine($"\nTotal de produtos: {produtos.Count}");
            return true;
        }

        public void BuscarProduto()
        {
            Console.WriteLine("=== BUSCAR PRODUTO ===\n");
            Console.Write("Digite o nome ou parte do nome: ");
            string termo = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(termo))
            {
                Console.WriteLine("Termo de busca inválido.");
                return;
            }

            var resultados = produtos.Values
                .Where(p => p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (resultados.Count == 0)
            {
                Console.WriteLine("Nenhum produto encontrado.");
                return;
            }

            Console.WriteLine($"\nEncontrados {resultados.Count} produto(s):\n");
            foreach (var p in resultados)
            {
                Console.WriteLine(p);
            }
        }

        public void EditarProduto()
        {
            Console.WriteLine("=== EDITAR PRODUTO ===\n");

            if (!ListarProdutos())
                return;

            int id = InputValidator.LerInteiroPositivo("\nDigite o ID do produto a editar");

            if (!produtos.TryGetValue(id, out var produto))
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            Console.WriteLine($"\nEditando: {produto.Nome}");
            Console.WriteLine("[1] Alterar nome");
            Console.WriteLine("[2] Alterar preço de custo");
            Console.WriteLine("[3] Alterar preço de venda");
            Console.WriteLine("[0] Cancelar");
            Console.Write("\nEscolha: ");

            if (!int.TryParse(Console.ReadLine(), out int opcao))
                return;

            switch (opcao)
            {
                case 1:
                    produto.Nome = InputValidator.LerTextoObrigatorio("Novo nome");
                    break;
                case 2:
                    produto.PrecoCusto = InputValidator.LerDecimalPositivo("Novo preço de custo");
                    break;
                case 3:
                    produto.PrecoVenda = InputValidator.LerDecimalMaiorQue("Novo preço de venda", produto.PrecoCusto);
                    break;
                case 0:
                    Console.WriteLine("Edição cancelada.");
                    return;
                default:
                    Console.WriteLine("Opção inválida.");
                    return;
            }

            ConsoleHelper.ExibirSucesso("Produto atualizado com sucesso!");
        }

        public void ExcluirProduto()
        {
            Console.WriteLine("=== EXCLUIR PRODUTO ===\n");

            if (!ListarProdutos())
                return;

            int id = InputValidator.LerInteiroPositivo("\nDigite o ID do produto a excluir");

            if (!produtos.TryGetValue(id, out var produto))
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            if (produto.QuantidadeEstoque > 0)
            {
                ConsoleHelper.ExibirAviso($"Este produto ainda possui {produto.QuantidadeEstoque} unidades em estoque!");
            }

            if (!InputValidator.ConfirmarAcao($"\nConfirma a exclusão de '{produto.Nome}'?"))
            {
                Console.WriteLine("Exclusão cancelada.");
                return;
            }

            produtos.Remove(id);

            ConsoleHelper.ExibirSucesso("Produto excluído com sucesso!");
        }

        public void MostrarRelatorios()
        {
            Console.WriteLine("=== RELATÓRIOS ===\n");

            Console.WriteLine($"Lucro Total Acumulado: {lucroTotal:C}");
            Console.WriteLine($"Total de Produtos Cadastrados: {produtos.Count}");
            Console.WriteLine($"Total de Transações: {transacoes.Count}");

            if (produtos.Count > 0)
            {
                var totalEstoque = produtos.Values.Sum(p => p.QuantidadeEstoque);
                var valorTotalEstoque = produtos.Values.Sum(p => p.PrecoCusto * p.QuantidadeEstoque);

                Console.WriteLine($"Total de Itens em Estoque: {totalEstoque}");
                Console.WriteLine($"Valor Total Investido em Estoque: {valorTotalEstoque:C}");

                // Produto com maior margem
                var maiorMargem = produtos.Values.OrderByDescending(p => p.MargemLucro).First();
                Console.WriteLine($"\nProduto com Maior Margem: {maiorMargem.Nome} ({maiorMargem.MargemLucro:F1}%)");

                // Produtos com estoque baixo
                var estoqueBaixo = produtos.Values.Where(p => p.QuantidadeEstoque <= 5).ToList();
                if (estoqueBaixo.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n⚠ Produtos com Estoque Baixo (≤5):");
                    foreach (var p in estoqueBaixo)
                    {
                        Console.WriteLine($"  - {p.Nome}: {p.QuantidadeEstoque} unidades");
                    }
                    Console.ResetColor();
                }
            }

            if (transacoes.Any())
            {
                var vendas = transacoes.Where(t => t.Tipo == TipoTransacao.Saida).ToList();
                if (vendas.Any())
                {
                    var produtoMaisVendido = vendas
                        .GroupBy(t => t.NomeProduto)
                        .OrderByDescending(g => g.Sum(t => t.Quantidade))
                        .First();

                    Console.WriteLine($"\nProduto Mais Vendido: {produtoMaisVendido.Key} " +
                                    $"({produtoMaisVendido.Sum(t => t.Quantidade)} unidades)");
                }
            }
        }

        public void MostrarHistorico()
        {
            Console.WriteLine("=== HISTÓRICO DE TRANSAÇÕES ===\n");

            if (transacoes.Count == 0)
            {
                Console.WriteLine("Nenhuma transação registrada.");
                return;
            }

            var ultimas = transacoes.OrderByDescending(t => t.Data).Take(20).ToList();

            Console.WriteLine($"Exibindo as últimas {ultimas.Count} transações:\n");

            foreach (var t in ultimas)
            {
                Console.WriteLine(t);
            }

            Console.WriteLine($"\nTotal de transações: {transacoes.Count}");
        }
    }
}