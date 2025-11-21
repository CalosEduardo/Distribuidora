using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DistribuidoraApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Distribuidora distribuidora = new Distribuidora();

            // Carrega os dados ao iniciar
            distribuidora.CarregarDados();

            int opcao = 0;
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║     SISTEMA DE CONTROLE - DISTRIBUIDORA  ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.WriteLine("\n[1] Cadastrar novo produto");
                Console.WriteLine("[2] Entrada de estoque");
                Console.WriteLine("[3] Saída de estoque (venda)");
                Console.WriteLine("[4] Listar produtos em estoque");
                Console.WriteLine("[5] Mostrar lucro total");
                Console.WriteLine("[0] Sair");
                Console.Write("\nEscolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nEntrada inválida! Digite um número de 0 a 5.");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    continue;
                }

                Console.Clear();

                switch (opcao)
                {
                    case 1:
                        distribuidora.CadastrarProduto();
                        break;
                    case 2:
                        distribuidora.EntradaEstoque();
                        break;
                    case 3:
                        distribuidora.SaidaEstoque();
                        break;
                    case 4:
                        distribuidora.ListarProdutos();
                        break;
                    case 5:
                        distribuidora.MostrarLucro();
                        break;
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nSalvando dados e saindo...");
                        distribuidora.SalvarDados();
                        Console.ResetColor();
                        Thread.Sleep(1000);
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nOpção inválida! Tente novamente.");
                        Console.ResetColor();
                        Thread.Sleep(1500);
                        break;
                }

                // Após qualquer operação, salva o estado atual
                distribuidora.SalvarDados();

                Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
                Console.ReadKey();
            }
        }
    }

    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int QuantidadeEstoque { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }

        public Produto(int id, string nome, int quantidade, decimal precoCusto, decimal precoVenda)
        {
            Id = id;
            Nome = nome;
            QuantidadeEstoque = quantidade;
            PrecoCusto = precoCusto;
            PrecoVenda = precoVenda;
        }

        public override string ToString()
        {
            return $"ID: {Id} | {Nome} | Estoque: {QuantidadeEstoque} | Custo: {PrecoCusto:C} | Venda: {PrecoVenda:C}";
        }
    }

    public class Distribuidora
    {
        private List<Produto> produtos = new List<Produto>();
        private decimal lucroTotal = 0;
        private int proximoId = 1;
        private const string caminhoArquivo = "produtos.txt";

        public void CadastrarProduto()
        {
            Console.Write("Nome do produto: ");
            string nome = Console.ReadLine();

            int quantidade;
            while (true)
            {
                Console.Write("Quantidade inicial: ");
                if (int.TryParse(Console.ReadLine(), out quantidade) && quantidade >= 0)
                    break;
                Console.WriteLine("Quantidade inválida! Digite um número inteiro não negativo.");
            }

            decimal precoCusto;
            while (true)
            {
                Console.Write("Preço de custo: ");
                string entrada = Console.ReadLine().Replace('.', ',');
                if (decimal.TryParse(entrada, out precoCusto) && precoCusto >= 0)
                    break;
                Console.WriteLine("Valor inválido! Use números válidos (ex: 12,50).");
            }

            decimal precoVenda;
            while (true)
            {
                Console.Write("Preço de venda: ");
                string entrada = Console.ReadLine().Replace('.', ',');
                if (decimal.TryParse(entrada, out precoVenda) && precoVenda > precoCusto)
                    break;
                Console.WriteLine("Preço de venda inválido! Deve ser maior que o custo.");
            }

            produtos.Add(new Produto(proximoId++, nome, quantidade, precoCusto, precoVenda));
            Console.WriteLine("Produto cadastrado com sucesso!");
        }

        public void EntradaEstoque()
        {
            ListarProdutos();

            Console.Write("\nDigite o ID do produto para entrada: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            Console.Write("Quantidade a adicionar: ");
            if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
            {
                Console.WriteLine("Quantidade inválida!");
                return;
            }

            produto.QuantidadeEstoque += quantidade;
            Console.WriteLine("Estoque atualizado.");
        }

        public void SaidaEstoque()
        {
            ListarProdutos();

            Console.Write("\nDigite o ID do produto para venda: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            Console.Write("Quantidade a vender: ");
            if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
            {
                Console.WriteLine("Quantidade inválida!");
                return;
            }

            if (quantidade > produto.QuantidadeEstoque)
            {
                Console.WriteLine("Estoque insuficiente.");
                return;
            }

            produto.QuantidadeEstoque -= quantidade;
            decimal lucro = (produto.PrecoVenda - produto.PrecoCusto) * quantidade;
            lucroTotal += lucro;

            Console.WriteLine($"Venda realizada. Lucro obtido: {lucro:C}");
        }

        public void ListarProdutos()
        {
            Console.WriteLine("\n--- Produtos em Estoque ---");
            if (produtos.Count == 0)
            {
                Console.WriteLine("Nenhum produto cadastrado.");
                return;
            }

            foreach (var p in produtos)
            {
                Console.WriteLine(p);
            }
        }

        public void MostrarLucro()
        {
            Console.WriteLine($"\nLucro total obtido: {lucroTotal:C}");
        }

        // ============= ARQUIVO TXT =============

        public void SalvarDados()
        {
            using (StreamWriter writer = new StreamWriter(caminhoArquivo, false))
            {
                foreach (var p in produtos)
                {
                    writer.WriteLine($"{p.Id}|{p.Nome}|{p.QuantidadeEstoque}|{p.PrecoCusto}|{p.PrecoVenda}");
                }
            }
        }

        public void CarregarDados()
        {
            if (!File.Exists(caminhoArquivo))
                return;

            produtos.Clear();
            var linhas = File.ReadAllLines(caminhoArquivo);

            foreach (var linha in linhas)
            {
                var partes = linha.Split('|');
                if (partes.Length == 5)
                {
                    int id = int.Parse(partes[0]);
                    string nome = partes[1];
                    int qtd = int.Parse(partes[2]);
                    decimal custo = decimal.Parse(partes[3]);
                    decimal venda = decimal.Parse(partes[4]);

                    produtos.Add(new Produto(id, nome, qtd, custo, venda));
                    proximoId = Math.Max(proximoId, id + 1);
                }
            }
        }
    }
}
