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
                // Exemplo: executa o teste de login
                var loginTest = new Testes.LoginTest(automator, logger);
                loginTest.Executar();

                // Aqui você pode chamar outros testes (CadastroClienteTest, FechamentoCaixaTest, etc.)
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