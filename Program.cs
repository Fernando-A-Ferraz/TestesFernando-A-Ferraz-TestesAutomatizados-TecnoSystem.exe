using System;

namespace TestesAutomatizados
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Utils.TestLogger("Logs/testes_log.txt");
            var automator = new TecnoSystemAutomator(logger);

            try
            {
                //executa o teste de login
                var loginTest = new Testes.LoginTest(automator);
                loginTest.Executar();

                //executa o teste de emissão de nota fiscal com múltiplas finalizadoras
                logger.Log("Iniciando teste de emissão de nota fiscal com múltiplas finalizadoras...");
                var emissaoNotaTest = new Testes.EmissaoNotaFiscalMultiplasFinalizadorasTest(automator);
                emissaoNotaTest.Executar();
            }
            catch (Exception ex)
            {
                logger.Log($"[FATAL] Erro inesperado: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                automator.FecharAplicacao();
            }
        }
    }
}