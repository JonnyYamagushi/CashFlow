# CashFlow

## Descrição
CashFlow é uma API desenvolvida em C# como parte da formação em .NET da Rocketseat, cujo objetivo é fornecer uma base para o gerenciamento de despesas, permitindo o registro e consulta de transações financeiras.

A solução segue uma arquitetura em camadas, com projetos separados para domínio (`CashFlow.Domain`), aplicação (`CashFlow.Application`), infraestrutura (`CashFlow.Infrastructure`), tratamento de exceções (`CashFlow.Exception`), comunicação (`CashFlow.Communication`) e expondo os endpoints REST através do projeto `CashFlow.API`.

Inclui ainda projetos de testes unitários (`tests/Validators.Tests` e `tests/CommonTestUtilities`) para garantir a qualidade da lógica de negócio e facilitar a criação de utilitários de teste.

## Tecnologias
- .NET 8.0
- C#
- Entity Framework Core (opcional)
- Swagger (Swashbuckle)
- xUnit / MSTest

## Download
Para baixar o código-fonte do repositório, execute:

```bash
git clone https://github.com/JonnyYamagushi/CashFlow.git
cd CashFlow
````

Ou faça o download direto como ZIP via GitHub:

1. Acesse [https://github.com/JonnyYamagushi/CashFlow](https://github.com/JonnyYamagushi/CashFlow)
2. Clique em **Code**
3. Selecione **Download ZIP**

## Instalação

1. Restaure os pacotes NuGet:
   ```bash
   dotnet restore
   ```

## Configuração

1. Abra `src/CashFlow.API/appsettings.Development.json` e configure a string de conexão do seu banco de dados.
2. Ajuste outras configurações de logging ou serviços conforme necessário.

## Execução

```bash
dotnet build
dotnet run --project src/CashFlow.API/CashFlow.API.csproj
```

A API estará disponível em:

- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

A documentação interativa (Swagger) fica em `https://localhost:5001/swagger`.

## Testes

Para executar todos os testes unitários:

```bash
dotnet test
```

## Licença

Este projeto está licenciado sob a MIT License.
Sinta-se livre para usar, estudar, modificar e distribuir.
