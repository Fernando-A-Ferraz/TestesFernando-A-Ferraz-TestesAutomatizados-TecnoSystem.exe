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

        // =================================================================================
        // MÉTODO PRINCIPAL DO TESTE
        // =================================================================================
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
                // SEU CÓDIGO ORIGINAL - NAVEGAÇÃO E ABERTURA DAS JANELAS (está perfeito)
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
                while (gerenciadorWindow == null && retries > 0)
                {
                    gerenciadorWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title == tituloGerenciadorWindow);
                    if (gerenciadorWindow != null) break;
                    Thread.Sleep(500);
                    retries--;
                }
                if (gerenciadorWindow == null) { _automator.Logger?.Log($"ERRO: Janela '{tituloGerenciadorWindow}' NÃO encontrada."); return; }
                _automator.Logger?.Log($"Janela '{tituloGerenciadorWindow}' encontrada!");
                gerenciadorWindow.Focus();
                Thread.Sleep(500);

                var btnNovaNFe = gerenciadorWindow.FindFirstDescendant(cf.ByName("Nova NFe").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnNovaNFe == null) { _automator.Logger?.Log("ERRO: Botão 'Nova NFe' NÃO encontrado."); return; }
                btnNovaNFe.Click();
                Thread.Sleep(500);
                
                string tituloEmissaoNFe = "...::: Emissão Nota Fiscal Eletrônica :::...";
                Window? emissaoNFeWindow = null;
                retries = 10;
                while (emissaoNFeWindow == null && retries > 0)
                {
                    emissaoNFeWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title == tituloEmissaoNFe);
                    if (emissaoNFeWindow != null) break;
                    Thread.Sleep(500);
                    retries--;
                }
                if (emissaoNFeWindow == null) { _automator.Logger?.Log($"ERRO: Janela '{tituloEmissaoNFe}' NÃO encontrada."); return; }
                _automator.Logger?.Log($"Janela '{tituloEmissaoNFe}' encontrada!");
                emissaoNFeWindow.Focus();
                Thread.Sleep(500);

                var tabCabecalho = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Cabeçalho").And(cf.ByControlType(ControlType.TabItem)))?.AsTabItem();
                if (tabCabecalho == null) { _automator.Logger?.Log("Aba 'Cabeçalho' não encontrada."); return; }

                var editNaturezaOp = FindTextBoxByLabel(tabCabecalho, "Natureza Operação:", cf);
                editNaturezaOp?.Enter("2");
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(500);

                var editClienteCod = FindTextBoxByLabel(tabCabecalho, "Código:", cf);
                editClienteCod?.Enter("102");
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(1000);

             

                // 1. Selecionar aba Produtos
                var tabProdutos = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Produtos").And(cf.ByControlType(ControlType.TabItem)))?.AsTabItem();
                if (tabProdutos == null) { _automator.Logger?.Log("ERRO: Aba 'Produtos' NÃO encontrada."); return; }
                tabProdutos.Select();
                Thread.Sleep(500);
                _automator.Logger?.Log("Aba 'Produtos' selecionada com sucesso.");

                // 2. Clicar em "Novo Produto"
                var btnNovoProduto = tabProdutos.FindFirstDescendant(cf.ByName("Novo Produto").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnNovoProduto == null) { _automator.Logger?.Log("ERRO: Botão 'Novo Produto' não encontrado."); return; }
                btnNovoProduto.Click();
                Thread.Sleep(500);

                // 3. Preencher Código do Produto
                var txtCodigoProduto = FindTextBoxByLabel(tabProdutos, "Código:", cf);
                if (txtCodigoProduto == null) { _automator.Logger?.Log("ERRO: Campo 'Código' do produto não encontrado."); return; }
                txtCodigoProduto.Enter("1");
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(1000);

                // 4. Preencher Valor Unitário
                var txtValorUnitario = FindTextBoxByLabel(tabProdutos, "Valor Unitário:", cf);
                if (txtValorUnitario == null) { _automator.Logger?.Log("ERRO: Campo 'Valor Unitário' do produto não encontrado."); return; }
                txtValorUnitario.Enter("200");
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(500);
                
                // 5. Clicar em "Salvar Produto"
                var btnSalvarProduto = tabProdutos.FindFirstDescendant(cf.ByName("Salvar Produto").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnSalvarProduto == null) { _automator.Logger?.Log("ERRO: Botão 'Salvar Produto' não encontrado."); return; }
                btnSalvarProduto.Click();
                Thread.Sleep(1000);

                // 6. Encontrar a janela de aviso "Produto salvo com sucesso!" e clicar em OK
                _automator.Logger?.Log("Aguardando janela de confirmação...");
                // <<-- IMPORTANTE: Verifique se o título da janela é "Aviso" ou algo diferente
                string tituloJanelaAviso = "Aviso"; 
                Window? avisoWindow = null;
                retries = 10;
                while (avisoWindow == null && retries > 0)
                {
                    // A busca é feita exatamente como você fez para as outras janelas
                    avisoWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title.Contains(tituloJanelaAviso));
                    if (avisoWindow != null) break;
                    Thread.Sleep(500);
                    retries--;
                }
                
                if (avisoWindow == null) { _automator.Logger?.Log($"ERRO: Janela de confirmação '{tituloJanelaAviso}' não encontrada."); return; }
                
                _automator.Logger?.Log("Janela de confirmação encontrada!");
                avisoWindow.Focus();
                
                // O botão OK tem AutomationId, então é mais fácil de achar
                var btnOk = avisoWindow.FindFirstDescendant(cf.ByAutomationId("BtnOk"))?.AsButton();
                if (btnOk == null) { _automator.Logger?.Log("ERRO: Botão 'Ok' (BtnOk) na janela de aviso não encontrado."); return; }
                btnOk.Click();
                _automator.Logger?.Log("Botão 'Ok' clicado. Produto salvo com sucesso!");
            }
            catch (Exception ex)
            {
                _automator.Logger?.Log($"[FATAL] ERRO NO TESTE: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        // Este método auxiliar é útil e não causa erros. Ele apenas organiza
        // a lógica de encontrar um campo pelo seu texto de rótulo.
        private TextBox? FindTextBoxByLabel(AutomationElement container, string labelText, ConditionFactory cf)
        {
            var label = container.FindFirstDescendant(cf.ByName(labelText).And(cf.ByControlType(ControlType.Text)));
            if (label == null) return null;

            var parent = label.Parent;
            if (parent != null)
            {
                var children = parent.FindAllChildren();
                bool labelFound = false;
                foreach (var child in children)
                {
                    if (labelFound && child.ControlType == ControlType.Edit) return child.AsTextBox();
                    if (child.Equals(label)) labelFound = true;
                }
            }
            return null;
        }
    }
}