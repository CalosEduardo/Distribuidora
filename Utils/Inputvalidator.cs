using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribuidora.Utils
{
    public static class InputValidator
    {
        public static string LerTextoObrigatorio(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string valor = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(valor) && valor.Length <= 100)
                    return valor;

                Console.WriteLine("Texto inválido! Deve ter entre 1 e 100 caracteres.");
            }
        }

        public static int LerInteiroPositivo(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (int.TryParse(Console.ReadLine(), out int valor) && valor > 0)
                    return valor;

                Console.WriteLine("Valor inválido! Digite um número inteiro positivo.");
            }
        }

        public static decimal LerDecimalPositivo(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string entrada = Console.ReadLine().Replace('.', ',');

                if (decimal.TryParse(entrada, out decimal valor) && valor >= 0)
                    return valor;

                Console.WriteLine("Valor inválido! Use números válidos (ex: 12,50).");
            }
        }

        public static decimal LerDecimalMaiorQue(string prompt, decimal minimo)
        {
            while (true)
            {
                Console.Write($"{prompt} (maior que {minimo:C}): ");
                string entrada = Console.ReadLine().Replace('.', ',');

                if (decimal.TryParse(entrada, out decimal valor) && valor > minimo)
                    return valor;

                Console.WriteLine($"Valor inválido! Deve ser maior que {minimo:C}.");
            }
        }

        public static bool ConfirmarAcao(string mensagem)
        {
            Console.Write($"{mensagem} (S/N): ");
            return Console.ReadLine()?.Trim().Equals("S", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
