using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribuidora.Utils
{
    public static class ConsoleHelper
    {
        public static void ExibirCabecalho()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║     SISTEMA DE CONTROLE - DISTRIBUIDORA  ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();
        }

        public static void ExibirErro(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{mensagem}");
            Console.ResetColor();
            Thread.Sleep(1500);
        }

        public static void ExibirSucesso(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✓ {mensagem}");
            Console.ResetColor();
        }

        public static void ExibirAviso(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n⚠ {mensagem}");
            Console.ResetColor();
        }
    }
}
