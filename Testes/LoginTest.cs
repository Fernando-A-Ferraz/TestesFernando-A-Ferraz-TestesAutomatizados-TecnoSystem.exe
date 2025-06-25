using System;
using FlaUI.Core.AutomationElements;

namespace TestesAutomatizados.Testes
{
    public class LoginTest
    {
        private readonly TecnoSystemAutomator _automator;
        private readonly Utils.TestLogger _logger;

        public LoginTest(TecnoSystemAutomator automator, Utils.TestLogger logger)
        {
            _automator = automator;
            _logger = logger;
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