// Arquivo: Program.cs

using System;
using TestesAutomatizados.Testes; // Essencial para encontrar as classes de teste
using TestesAutomatizados.Utils; // Para encontrar a classe TestLogger

namespace TestesAutomatizados
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new TestLogger("Logs/testes_log.txt");
            var automator = new TecnoSystemAutomator(logger);

            try
            {
                // 1. Executa o teste de login
                logger.Log("Iniciando teste de Login...");
                var loginTest = new LoginTest(automator);
                loginTest.Executar();
                logger.Log("Teste de Login concluído.");

                // 2. Executa o teste de emissão de nota fiscal
               // logger.Log("Iniciando teste de emissão de nota fiscal com múltiplas finalizadoras...");
                var emissaoNotaTest = new EmissaoNotaFiscalMultiplasFinalizadorasTest(automator);
                emissaoNotaTest.Executar();
                logger.Log("Teste de emissão de nota fiscal concluído.");
            }
            catch (Exception ex)
            {
                logger.Log($"[FATAL] Erro inesperado no orquestrador principal: {ex.Message}\n{ex.StackTrace}");
            }
           finally
            {
                automator.FecharAplicacao();
                Console.WriteLine("Todos os testes foram finalizados. Pressione qualquer tecla para fechar.");
                Console.ReadKey();
            }
        }
    }
}