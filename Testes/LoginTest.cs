using System;

namespace TestesAutomatizados.Testes
{
    public class LoginTest
    {
        private readonly TecnoSystemAutomator _automator;
        private readonly Utils.TestLogger _logger;

        public LoginTest(TecnoSystemAutomator automator)
        {
            string testName = GetType().Name;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logFileName = $"Logs/{testName}_{timestamp}.log";
            _logger = new Utils.TestLogger(logFileName);
            _automator = automator;
        }

        public void Executar()
        {
            _logger.Log("Iniciando teste de login...");

            var mainWindow = _automator.AbrirELogar();

            if (mainWindow != null)
            {
                _logger.Log("Login bem-sucedido! Tela principal encontrada.");
            }
            else
            {
                _logger.Log("Falha no login ou não foi possível encontrar a janela principal.");
            }
        }
    }
}