using System;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.UIA2;
using System.Threading;
using FlaUI.Core.WindowsAPI;

namespace TestesAutomatizados
{
    public class TecnoSystemAutomator
    {
        private Application? _app;
        private UIA2Automation _automation;
        public ConditionFactory _cf;
        private readonly Utils.TestLogger? _logger;

        public string CaminhoDaAplicacao { get; set; } = @"D:\TecnoSystem\TecnoSystem.exe";
        public string TituloLoginWindow { get; set; } = "...::: TecnoSystem :::...";
        public string Usuario { get; set; } = "1";
        public string Senha { get; set; } = "572600";
        public string ParteTituloJanelaPrincipal { get; set; } = "TecnoSystem Versão";

        private Window? _mainWindow; // <--- Armazene a janela principal

        public TecnoSystemAutomator(Utils.TestLogger? logger = null)
        {
            _automation = new UIA2Automation();
            _cf = new ConditionFactory(_automation.PropertyLibrary);
            _logger = logger;
        }

        /// <summary>
        /// Abre e loga apenas se ainda não estiver aberto/logado.
        /// Sempre retorna a mainWindow já logada.
        /// </summary>
        public Window? AbrirELogar()
        {
            if (_mainWindow != null && !_mainWindow.IsOffscreen)
            {
                // Já estamos logados e com a janela aberta!
                return _mainWindow;
            }

            try
            {
                _logger?.Log("Tentando iniciar a aplicação TecnoSystem...");
                _app = Application.Launch(CaminhoDaAplicacao);
                Thread.Sleep(5000);

                Window? loginWindow = null;
                var initialWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(15));

                if (initialWindow != null && initialWindow.Title == TituloLoginWindow)
                {
                    loginWindow = initialWindow.AsWindow();
                    _logger?.Log($"Janela de login encontrada via GetMainWindow: '{loginWindow.Title}'");
                }
                else
                {
                    _logger?.Log($"GetMainWindow retornou '{initialWindow?.Title}', que não é a janela de login esperada ('{TituloLoginWindow}'). Buscando no desktop...");
                    var desktop = _automation.GetDesktop();
                    loginWindow = desktop.FindFirstChild(_cf.ByControlType(ControlType.Window).And(_cf.ByName(TituloLoginWindow)))?.AsWindow();
                }

                if (loginWindow == null)
                {
                    _logger?.Log($"Janela de login com título '{TituloLoginWindow}' não encontrada.");
                    return null;
                }

                loginWindow.Focus();
                Thread.Sleep(500);

                var txtUsuario = loginWindow.FindFirstDescendant(_cf.ByAutomationId("TxtUsuarioCodigo"))?.AsTextBox();
                if (txtUsuario == null) { _logger?.Log("Campo Usuário não encontrado."); return null; }
                txtUsuario.Text = Usuario;
                Thread.Sleep(100);
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(700);

                var txtSenha = loginWindow.FindFirstDescendant(_cf.ByClassName("PasswordBox"))?.AsTextBox();
                if (txtSenha == null)
                {
                    var allEdits = loginWindow.FindAllDescendants(_cf.ByControlType(ControlType.Edit));
                    if (allEdits.Length > 1 && allEdits[0].AutomationId == "TxtUsuarioCodigo")
                        txtSenha = allEdits[1].AsTextBox();
                    else if (allEdits.Length == 1 && allEdits[0].AutomationId != "TxtUsuarioCodigo")
                        txtSenha = allEdits[0].AsTextBox();
                }
                if (txtSenha == null) { _logger?.Log("Campo Senha não encontrado."); return null; }
                txtSenha.Focus();
                txtSenha.Text = Senha;
                Thread.Sleep(500);

                var btnAcessarSistema = loginWindow.FindFirstDescendant(_cf.ByName("Acessar Sistema").And(_cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnAcessarSistema == null) { _logger?.Log("Botão 'Acessar Sistema' não encontrado."); return null; }
                btnAcessarSistema.Click();
                Thread.Sleep(5000);

                Window? mainWindow = null;
                IntPtr loginWindowHandle = loginWindow.Properties.NativeWindowHandle.ValueOrDefault;
                int retries = 10;
                while (mainWindow == null && retries > 0)
                {
                    Thread.Sleep(1500);
                    var allAppWindows = _app.GetAllTopLevelWindows(_automation);
                    foreach (var currentWindowElement in allAppWindows)
                    {
                        Window? currentWindow = currentWindowElement.AsWindow();
                        if (currentWindow == null) continue;
                        IntPtr currentWindowHandle = currentWindow.Properties.NativeWindowHandle.ValueOrDefault;
                        if (currentWindow.Title.Contains(ParteTituloJanelaPrincipal) &&
                            (loginWindowHandle == IntPtr.Zero || currentWindowHandle != loginWindowHandle))
                        {
                            mainWindow = currentWindow;
                            break;
                        }
                    }
                    if (mainWindow != null) break;
                    retries--;
                }

                if (mainWindow != null)
                {
                    mainWindow.Focus();
                    _logger?.Log($"Janela principal (pós-login) encontrada: '{mainWindow.Title}'");
                    _mainWindow = mainWindow; // <-- Armazene a janela principal!
                    return mainWindow;
                }
                else
                {
                    _logger?.Log($"Janela principal (pós-login) com título contendo '{ParteTituloJanelaPrincipal}' não encontrada.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger?.Log($"Ocorreu um erro em AbrirELogar: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Retorna a janela principal já logada, se existir.
        /// </summary>
        public Window? MainWindow => _mainWindow;

        public void FecharAplicacao()
        {
            _logger?.Log("Tentando fechar a aplicação TecnoSystem...");
            _app?.Close();
            _app?.Dispose();
            _automation?.Dispose();
            _mainWindow = null;
            _logger?.Log("Aplicação e recursos de automação liberados.");
        }
    }
}