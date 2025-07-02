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
                
                #region Setup da Nota Fiscal
                // Inicia a navegação pelos menus para chegar na tela de emissão de NF-e
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
                Thread.Sleep(500);

                // Aguarda e foca na janela do Gerenciador de NF-e
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
                gerenciadorWindow.Focus();
                Thread.Sleep(500);

                // Clica no botão para criar uma nova nota fiscal
                var btnNovaNFe = gerenciadorWindow.FindFirstDescendant(cf.ByName("Nova NFe").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnNovaNFe == null) { _automator.Logger?.Log("ERRO: Botão 'Nova NFe' NÃO encontrado."); return; }
                btnNovaNFe.Click();
                Thread.Sleep(500);
                
                // Aguarda e foca na janela de Emissão de NF-e
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
                emissaoNFeWindow.Focus();
                Thread.Sleep(500);
                
                // Preenche os dados do cabeçalho
                var tabCabecalho = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Cabeçalho").And(cf.ByControlType(ControlType.TabItem)))?.AsTabItem();
                if (tabCabecalho == null) { _automator.Logger?.Log("Aba 'Cabeçalho' não encontrada."); return; }

                var editNaturezaOp = FindTextBoxByLabel(tabCabecalho, "Natureza Operação:", cf);
                editNaturezaOp?.Enter("2");
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(500);

                var editClienteCod = FindTextBoxByLabel(tabCabecalho, "Código:", cf);
                editClienteCod?.Enter("102");
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(500);

                // Seleciona a aba de produtos e adiciona um novo item
                var tabProdutos = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Produtos").And(cf.ByControlType(ControlType.TabItem)))?.AsTabItem();
                if (tabProdutos == null) { _automator.Logger?.Log("ERRO: Aba 'Produtos' NÃO encontrada."); return; }
                tabProdutos.Select();
                Thread.Sleep(500);

                var btnNovoProduto = tabProdutos.FindFirstDescendant(cf.ByName("Novo Produto").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnNovoProduto == null) { _automator.Logger?.Log("ERRO: Botão 'Novo Produto' não encontrado."); return; }
                btnNovoProduto.Click();
                Thread.Sleep(500);

                var txtCodigoProduto = FindTextBoxByLabel(tabProdutos, "Código:", cf);
                if (txtCodigoProduto == null) { _automator.Logger?.Log("ERRO: Campo 'Código' do produto não encontrado."); return; }
                txtCodigoProduto.Enter("1");
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(500);

                var txtValorUnitario = FindTextBoxByLabel(tabProdutos, "Valor Unitário:", cf);
                if (txtValorUnitario == null) { _automator.Logger?.Log("ERRO: Campo 'Valor Unitário' do produto não encontrado."); return; }
                txtValorUnitario.Enter("200");
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(500);
                
                var btnSalvarProduto = tabProdutos.FindFirstDescendant(cf.ByName("Salvar Produto").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnSalvarProduto == null) { _automator.Logger?.Log("ERRO: Botão 'Salvar Produto' não encontrado."); return; }
                btnSalvarProduto.Click();
                Thread.Sleep(500);
                
                // Lida com a janela de aviso após salvar o produto
                string tituloJanelaAvisoProduto = "Aviso";
                Window? avisoProdutoWindow = null;
                retries = 10;
                while (avisoProdutoWindow == null && retries > 0) { avisoProdutoWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title.Contains(tituloJanelaAvisoProduto)); if (avisoProdutoWindow != null) break; Thread.Sleep(500); retries--; }
                if (avisoProdutoWindow == null) { _automator.Logger?.Log($"ERRO: Janela de confirmação '{tituloJanelaAvisoProduto}' não encontrada."); return; }
                avisoProdutoWindow.Focus();
                var btnOkProduto = avisoProdutoWindow.FindFirstDescendant(cf.ByAutomationId("BtnOk"))?.AsButton();
                if (btnOkProduto == null) { _automator.Logger?.Log("ERRO: Botão 'Ok' (BtnOk) na janela de aviso do produto não encontrado."); return; }
                btnOkProduto.Click();
                
                // Seleciona a aba de transporte e define a modalidade do frete
                var tabTransporte = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Transporte").And(cf.ByControlType(ControlType.TabItem)))?.AsTabItem();
                if (tabTransporte == null) { _automator.Logger?.Log("ERRO: Aba 'Transporte' não encontrada."); return; }
                tabTransporte.Select();
                Thread.Sleep(500);
                
                string labelDoComboBox = "Modalidade do Frete:"; 
                var cmbModalidadeFrete = FindComboBoxByLabel(tabTransporte, labelDoComboBox, cf);
                if (cmbModalidadeFrete == null) { _automator.Logger?.Log("ERRO: ComboBox 'Modalidade de Frete' não encontrado."); return; }
                cmbModalidadeFrete.Select("9 - Sem Ocorrência de Transporte");
                Thread.Sleep(500);
                #endregion

                // Seleciona a aba "Cobrança" para iniciar o processo de finalização
                var tabCobranca = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Cobrança").And(cf.ByControlType(ControlType.TabItem)))?.AsTabItem();
                if (tabCobranca == null) { _automator.Logger?.Log("ERRO: Aba 'Cobrança' não encontrada."); return; }
                tabCobranca.Select();
                Thread.Sleep(500);
                _automator.Logger?.Log("Aba 'Cobrança' selecionada.");

                // 1. Preenche os dados da finalizadora na aba de cobrança
                _automator.Logger?.Log("Preenchendo a finalizadora...");
                var txtCodigoFinalizadora = FindTextBoxByLabel(tabCobranca, "Código Finalizadora:", cf);
                if (txtCodigoFinalizadora == null) { _automator.Logger?.Log("ERRO: Campo 'Código Finalizadora' não encontrado."); return; }
                txtCodigoFinalizadora.Enter("1"); // Insere o código da finalizadora
                Keyboard.Press(VirtualKeyShort.ENTER); // Pressiona Enter para confirmar a finalizadora
                Thread.Sleep(500);
                Keyboard.Press(VirtualKeyShort.ENTER); // Pressiona Enter no campo "Valor", que já está focado e com o valor correto
                Thread.Sleep(500);

                // 2. Clica no botão "Salvar" para registrar a nota fiscal
                _automator.Logger?.Log("Clicando no botão 'Salvar'...");
                var btnSalvar = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Salvar").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnSalvar == null) { _automator.Logger?.Log("ERRO: Botão 'Salvar' não encontrado."); return; }
                btnSalvar.Click();
                Thread.Sleep(500);

                // 3. Lida com o diálogo de confirmação "Não há transportador"
                _automator.Logger?.Log("Aguardando diálogo de confirmação do transportador...");
               
                string tituloDialogoTransportador = "Confirmação"; 
                Window? dialogoTransportador = null;
                retries = 10;
                // Procura a janela de diálogo pelo título "Confirmação" com várias tentativas
                while (dialogoTransportador == null && retries > 0) { dialogoTransportador = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title.Contains(tituloDialogoTransportador)); if (dialogoTransportador != null) break; Thread.Sleep(500); retries--; }
                if (dialogoTransportador == null) { _automator.Logger?.Log($"ERRO: Diálogo '{tituloDialogoTransportador}' não encontrado."); return; }
                dialogoTransportador.Focus();
                
                // Dentro do diálogo, localiza e clica no botão "Sim"
                var btnSim = dialogoTransportador.FindFirstDescendant(cf.ByAutomationId("BtnSim"))?.AsButton();
                if (btnSim == null) { _automator.Logger?.Log("ERRO: Botão 'Sim' (BtnSim) não encontrado."); return; }
                btnSim.Click();
                Thread.Sleep(500);

                // 4. Lida com o diálogo de sucesso "Nota Fiscal Eletrônica salva com sucesso!"
                _automator.Logger?.Log("Aguardando diálogo de sucesso...");
               
                string tituloDialogoSucesso = "Aviso"; 
                Window? dialogoSucesso = null;
                retries = 10;
                // Procura a janela de aviso pelo título "Aviso", garantindo que não é a mesma janela anterior
                while (dialogoSucesso == null && retries > 0) { dialogoSucesso = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title.Contains(tituloDialogoSucesso) && w.Properties.NativeWindowHandle != dialogoTransportador.Properties.NativeWindowHandle); if (dialogoSucesso != null) break; Thread.Sleep(500); retries--; }
                if (dialogoSucesso == null) { _automator.Logger?.Log($"ERRO: Diálogo de sucesso '{tituloDialogoSucesso}' não encontrado."); return; }
                dialogoSucesso.Focus();

                // Dentro do diálogo de sucesso, localiza e clica no botão "Ok"
                var btnOkSucesso = dialogoSucesso.FindFirstDescendant(cf.ByAutomationId("BtnOk"))?.AsButton();
                if (btnOkSucesso == null) { _automator.Logger?.Log("ERRO: Botão 'Ok' (BtnOk) de sucesso não encontrado."); return; }
                btnOkSucesso.Click();
                Thread.Sleep(500);

                // 5. Clica no botão "Sair" para fechar a janela de emissão de NF-e
                _automator.Logger?.Log("Clicando no botão 'Sair' para fechar a janela da NF-e...");
                var btnSair = emissaoNFeWindow.FindFirstDescendant(cf.ByName("Sair").And(cf.ByControlType(ControlType.Button)))?.AsButton();
                if (btnSair == null) { _automator.Logger?.Log("ERRO: Botão 'Sair' não encontrado."); return; }
                btnSair.Click();

                // Loga a conclusão bem-sucedida do teste
                _automator.Logger?.Log("!!! TESTE DE EMISSÃO DE NOTA COM FINALIZADORA ÚNICA CONCLUÍDO COM SUCESSO !!!");
            }
            catch (Exception ex)
            {
                _automator.Logger?.Log($"[FATAL] ERRO NO TESTE: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        #region Métodos Auxiliares
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

        private ComboBox? FindComboBoxByLabel(AutomationElement container, string labelText, ConditionFactory cf)
        {
            var label = container.FindFirstDescendant(cf.ByName(labelText).And(cf.ByControlType(ControlType.Text)));
            if (label == null) { _automator.Logger?.Log($"ERRO: Label '{labelText}' para o ComboBox não foi encontrado."); return null; }
            var parent = label.Parent;
            if (parent != null)
            {
                var children = parent.FindAllChildren();
                bool labelFound = false;
                foreach (var child in children)
                {
                    if (labelFound && child.ControlType == ControlType.ComboBox) return child.AsComboBox();
                    if (child.Equals(label)) labelFound = true;
                }
            }
            return null;
        }
        #endregion
    }
}