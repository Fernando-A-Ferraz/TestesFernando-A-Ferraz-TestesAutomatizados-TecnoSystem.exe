#nullable enable

using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.UIA2;
using System.Threading;
using System.Linq;
using FlaUI.Core;
using FlaUI.Core.WindowsAPI;

namespace TestesAutomatizados.Testes
{
    public class EmissaoNotaFiscalMultiplasFinalizadorasTest
    {
        private readonly TecnoSystemAutomator _automator;

        public EmissaoNotaFiscalMultiplasFinalizadorasTest(TecnoSystemAutomator automator)
        {
            _automator = automator;
        }

        public void Executar()
        {
            Window? mainWindow = _automator.MainWindow;
            if (mainWindow == null)
                throw new InvalidOperationException("A janela principal não foi encontrada. O teste de login pode ter falhado.");

            UIA2Automation automation = _automator._automation;
            ConditionFactory cf = _automator._cf;
            Application app = _automator._app!;

            try
            {
                _automator.Logger?.Log("Iniciando navegação no menu para emissão de NF-e...");
                var menuMovimentacoes = mainWindow.FindFirstDescendant(cf.ByAutomationId("MniMovimentacoes"))?.AsMenuItem();
                if (menuMovimentacoes == null) { _automator.Logger?.Log("ERRO: Menu 'Movimentações' não encontrado."); return; }
                menuMovimentacoes.Expand();
                Thread.Sleep(500);

                var menuNFe = mainWindow.FindFirstDescendant(cf.ByAutomationId("MniNFe"))?.AsMenuItem();
                if (menuNFe == null) { _automator.Logger?.Log("ERRO: Menu 'NFe' não encontrado."); return; }
                menuNFe.Expand();
                Thread.Sleep(500);

                var menuGerenciadorNFe = mainWindow.FindFirstDescendant(cf.ByAutomationId("MniGerenciadorNFe"))?.AsMenuItem();
                if (menuGerenciadorNFe == null) { _automator.Logger?.Log("ERRO: Menu 'Gerenciador de Notas Fiscais Eletrônicas' não encontrado."); return; }
                menuGerenciadorNFe.Invoke();
                Thread.Sleep(1500);

                string tituloGerenciadorWindow = "...::: Gerenciador de Notas Fiscais Eletrônicas :::...";
                Window? gerenciadorWindow = null;
                int retries = 10;
                TimeSpan delayEntreTentativas = TimeSpan.FromMilliseconds(500);

                while (gerenciadorWindow == null && retries > 0)
                {
                    gerenciadorWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title == tituloGerenciadorWindow)?.AsWindow();
                    if (gerenciadorWindow == null)
                    {
                        gerenciadorWindow = automation.GetDesktop().FindFirstChild(cf.ByName(tituloGerenciadorWindow).And(cf.ByControlType(ControlType.Window)))?.AsWindow();
                    }
                    if (gerenciadorWindow != null) break;
                    Thread.Sleep(delayEntreTentativas);
                    retries--;
                    _automator.Logger?.Log($"Tentando encontrar '{tituloGerenciadorWindow}', tentativas restantes: {retries}");
                }

                if (gerenciadorWindow == null) { _automator.Logger?.Log($"ERRO: Janela '{tituloGerenciadorWindow}' NÃO encontrada."); return; }
                _automator.Logger?.Log($"Janela '{tituloGerenciadorWindow}' encontrada!");
                gerenciadorWindow.Focus();
                Thread.Sleep(500);

                // Clica em Nova NFe
                var btnNovaNFe = gerenciadorWindow.FindFirstDescendant(cf.ByName("Nova NFe").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnNovaNFe == null) { _automator.Logger?.Log("ERRO: Botão 'Nova NFe' NÃO encontrado."); return; }
                btnNovaNFe.Click();
                Thread.Sleep(500);

                // Aguarda e encontra a janela correta de emissão
                string tituloEmissaoNFe = "...::: Emissão Nota Fiscal Eletrônica :::...";
                Window? emissaoNFeWindow = null;
                retries = 15;
                IntPtr gerenciadorHandle = gerenciadorWindow.Properties.NativeWindowHandle.ValueOrDefault;

                while (emissaoNFeWindow == null && retries > 0)
                {
                    emissaoNFeWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault(w =>
                        w.Title == tituloEmissaoNFe && (gerenciadorHandle == IntPtr.Zero || w.Properties.NativeWindowHandle.ValueOrDefault != gerenciadorHandle)
                    )?.AsWindow();

                    if (emissaoNFeWindow == null)
                    {
                        emissaoNFeWindow = automation.GetDesktop()
                            .FindFirstChild(cf.ByName(tituloEmissaoNFe).And(cf.ByControlType(ControlType.Window)))?.AsWindow();
                        if (emissaoNFeWindow != null && gerenciadorHandle != IntPtr.Zero && emissaoNFeWindow.Properties.NativeWindowHandle.ValueOrDefault == gerenciadorHandle)
                        {
                            emissaoNFeWindow = null;
                        }
                    }
                    if (emissaoNFeWindow != null) break;
                    Thread.Sleep(delayEntreTentativas);
                    retries--;
                    _automator.Logger?.Log($"Tentando encontrar '{tituloEmissaoNFe}', tentativas restantes: {retries}");
                }

                if (emissaoNFeWindow == null) { _automator.Logger?.Log($"ERRO: Janela '{tituloEmissaoNFe}' NÃO encontrada."); return; }
                _automator.Logger?.Log($"Janela '{tituloEmissaoNFe}' encontrada!");
                emissaoNFeWindow.Focus();
                Thread.Sleep(500);

                // Preencher aba Cabeçalho
                var tabCabecalho = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Cabeçalho").And(cf.ByControlType(ControlType.TabItem)))?.AsTabItem();
                if (tabCabecalho == null) { _automator.Logger?.Log("Aba 'Cabeçalho' não encontrada."); return; }
                _automator.Logger?.Log("Aba 'Cabeçalho' encontrada.");

                // Natureza Operação
                _automator.Logger?.Log("Tentando encontrar campo 'Natureza Operação'...");
                TextBox editNaturezaOp = null;
                {
                    var labelNaturezaOp = tabCabecalho.FindFirstDescendant(cf.ByName("Natureza Operação:").And(cf.ByControlType(ControlType.Text)));
                    if (labelNaturezaOp != null)
                    {
                        var parentOfLabelNatOp = labelNaturezaOp.Parent;
                        if (parentOfLabelNatOp != null)
                        {
                            var childrenOfParentNatOp = parentOfLabelNatOp.FindAllChildren();
                            bool labelNatOpFound = false;
                            foreach (var child in childrenOfParentNatOp)
                            {
                                if (labelNatOpFound && child.ControlType == ControlType.Edit)
                                {
                                    editNaturezaOp = child.AsTextBox();
                                    break;
                                }
                                if (child.Equals(labelNaturezaOp)) { labelNatOpFound = true; }
                            }
                        }
                    }
                }

                if (editNaturezaOp != null)
                {
                    _automator.Logger?.Log("Preenchendo Natureza Operação com '2'...");
                    editNaturezaOp.Text = "2";
                    Thread.Sleep(200);
                    Keyboard.Press(VirtualKeyShort.ENTER);
                    Thread.Sleep(500);
                    _automator.Logger?.Log("Natureza Operação preenchida.");
                }
                else { _automator.Logger?.Log("Campo 'Natureza Operação' NÃO encontrado. Não foi possível preencher."); }

                // Cliente/Fornecedor - Código
                _automator.Logger?.Log("Tentando encontrar campo 'Cliente/Fornecedor - Código'...");
                TextBox editClienteCod = null;
                {
                    var groupClienteFornecedor = tabCabecalho.FindFirstDescendant(cf.ByName("Cliente / Fornecedor").And(cf.ByControlType(ControlType.Group)));
                    if (groupClienteFornecedor != null)
                    {
                        var labelCodigoCliente = groupClienteFornecedor.FindFirstDescendant(cf.ByName("Código:").And(cf.ByControlType(ControlType.Text)));
                        if (labelCodigoCliente != null)
                        {
                            var parentOfLabelCodigo = labelCodigoCliente.Parent;
                            if (parentOfLabelCodigo != null)
                            {
                                var childrenOfParentCodigo = parentOfLabelCodigo.FindAllChildren();
                                bool labelCodigoFound = false;
                                foreach (var child in childrenOfParentCodigo)
                                {
                                    if (labelCodigoFound && child.ControlType == ControlType.Edit)
                                    {
                                        editClienteCod = child.AsTextBox();
                                        break;
                                    }
                                    if (child.Equals(labelCodigoCliente))
                                    {
                                        labelCodigoFound = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (editClienteCod != null)
                {
                    _automator.Logger?.Log("Preenchendo Cliente/Fornecedor Código com '102'...");
                    editClienteCod.Text = "102";
                    Thread.Sleep(200);
                    Keyboard.Press(VirtualKeyShort.ENTER);
                    Thread.Sleep(1000);
                    _automator.Logger?.Log("Cliente/Fornecedor Código preenchido.");
                }
                else { _automator.Logger?.Log("Campo 'Cliente/Fornecedor - Código' NÃO encontrado. Não foi possível preencher."); }

                // Selecionar aba Produtos
                var tabProdutos = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Produtos").And(cf.ByControlType(ControlType.TabItem)))?.AsTabItem();
                if (tabProdutos == null)
                {
                    _automator.Logger?.Log("ERRO: Aba 'Produtos' NÃO encontrada.");
                    return;
                }
                tabProdutos.Select();
                Thread.Sleep(1000);

                _automator.Logger?.Log("Campos preenchidos e aba Produtos selecionada com sucesso!");
                // ... continue sua lógica de teste a partir daqui ...
            }
            catch (Exception ex)
            {
                _automator.Logger?.Log($"[FATAL] ERRO NO TESTE TestesNfesComMultiplasFinalizadoras: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
    }
}