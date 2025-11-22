using Distribuidora.Repositories;
using Distribuidora.Utils;
using Distribuidora.Repositories;
using Distribuidora.Services;
using Distribuidora.Utils;
using System;
using System.Globalization;
using System.Threading;

namespace Distribuidora
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define cultura para garantir formatação correta
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            var repositorio = new RepositorioProdutos();
            var distribuidora = new DistribuidoraService(repositorio);

            distribuidora.CarregarDados();

            int opcao = 0;
            while (true)
            {
                Console.Clear();
                ConsoleHelper.ExibirCabecalho();

                Console.WriteLine("\n[1] Cadastrar novo produto");
                Console.WriteLine("[2] Entrada de estoque");
                Console.WriteLine("[3] Saída de estoque (venda)");
                Console.WriteLine("[4] Listar produtos em estoque");
                Console.WriteLine("[5] Buscar produto");
                Console.WriteLine("[6] Editar produto");
                Console.WriteLine("[7] Excluir produto");
                Console.WriteLine("[8] Mostrar relatórios");
                Console.WriteLine("[9] Histórico de transações");
                Console.WriteLine("[0] Sair");
                Console.Write("\nEscolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    ConsoleHelper.ExibirErro("Entrada inválida! Digite um número de 0 a 9.");
                    continue;
                }

                Console.Clear();

                try
                {
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
                            distribuidora.BuscarProduto();
                            break;
                        case 6:
                            distribuidora.EditarProduto();
                            break;
                        case 7:
                            distribuidora.ExcluirProduto();
                            break;
                        case 8:
                            distribuidora.MostrarRelatorios();
                            break;
                        case 9:
                            distribuidora.MostrarHistorico();
                            break;
                        case 0:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\nSalvando dados e saindo...");
                            distribuidora.SalvarDados();
                            Console.ResetColor();
                            Thread.Sleep(1000);
                            return;
                        default:
                            ConsoleHelper.ExibirErro("Opção inválida! Tente novamente.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.ExibirErro($"Erro: {ex.Message}");
                }

                distribuidora.SalvarDados();

                Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
                Console.ReadKey();
            }
        }
    }
}