# Testes Automatizados - TecnoSystem

## Objetivo dos Testes
Automatizar fluxos críticos do sistema TecnoSystem, garantindo a repetibilidade e confiabilidade nos testes de regressão e validação de funcionalidades.

## Ambiente de Teste
- Windows 10 ou superior
- .NET 6+
- Visual Studio Code
- FlaUI.Core e FlaUI.UIA2

## Estrutura do Projeto

```
/TestesAutomatizados
├── Program.cs
├── TecnoSystemAutomator.cs
├── Testes/
│   └── LoginTest.cs
├── Utils/
│   └── TestLogger.cs
│   └── AppHelper.cs
├── Logs/
│   └── testes_log.txt
```

## Ferramentas utilizadas
- **FlaUI:** Framework para automação de UI em aplicações Windows.
- **C# Console App:** Execução dos testes automatizados.
- **VS Code:** Edição e execução dos testes.

## Como Executar

1. Ajuste o caminho do executável no arquivo `TecnoSystemAutomator.cs`:
   ```csharp
   public string CaminhoDaAplicacao { get; set; } = @"D:\TecnoSystem\TecnoSystem.exe";
   ```

2. Execute no terminal:
   ```sh
   dotnet run
   ```

## Plano de Casos de Teste

| ID   | Nome do Teste                          | Pré-requisito        | Passos                                                                                                      | Resultado Esperado                                    |
|------|----------------------------------------|----------------------|-------------------------------------------------------------------------------------------------------------|-------------------------------------------------------|
| T01  | Login válido                           | App instalado        | - Abrir app<br>- Preencher usuário/senha<br>- Clicar em Login                                                | Tela inicial do sistema visível                       |
| T02  | Cadastro de cliente                    | Login realizado      | - Acessar menu Cadastro<br>- Preencher dados<br>- Salvar                                                     | Mensagem de sucesso, cliente cadastrado               |
| T03  | Fechamento de caixa                    | Login realizado      | - Abrir módulo Caixa<br>- Realizar fechamento<br>- Confirmar                                                 | Caixa fechado com sucesso                             |
| T04  | Emissão de nota fiscal                 | Login realizado      | - Acessar módulo de NF<br>- Preencher dados obrigatórios<br>- Emitir NF                                      | Nota fiscal emitida com sucesso                       |
| T05  | Múltiplas formas de pagamento variadas | NF emitida com êxito | - Selecionar vendas<br>- Definir formas de pagamento (crédito, débito, dinheiro, boleto e PIX)<br>- Confirmar | Pagamento registrado corretamente em múltiplas formas |


---

## Regras de Execução

- Todos os logs são gravados em `Logs/testes_log.txt`.
- Em caso de erro, revise os prints do console e o arquivo de log.
- Os testes podem ser expandidos em `/Testes`.

---

## Observações
- Adapte as credenciais e caminhos conforme necessário.
- Para novos testes, crie arquivos em `/Testes` seguindo o padrão `NomeTest.cs`.
