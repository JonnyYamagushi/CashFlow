# CashFlow

## Sumário

- [Descrição](#descrição)
- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Configuração](#instalação-e-configuração)
- [Executando a API](#executando-a-api)
- [Documentação Swagger](#documentação-swagger)
- [Endpoints da API](#endpoints-da-api)
  - [Transações (Expenses)](#transações-expenses)
  - [Relatórios (Report)](#relatórios-report)
- [Exemplos de Uso](#exemplos-de-uso)
- [Testes](#testes)
- [Contribuindo](#contribuindo)
- [Licença](#licença)

## Descrição

CashFlow é uma API desenvolvida em **C# (.NET 8.0)** como parte da formação em .NET da **[Rocketseat](https://www.rocketseat.com.br/)**. Seu objetivo é fornecer uma base sólida para o gerenciamento de despesas, permitindo o registro e a consulta de transações financeiras, bem como a ***geração de relatórios em Excel e PDF***.

A solução adota uma arquitetura em camadas baseada em **Domain-Driven Design (DDD)**, separando claramente as responsabilidades em projetos distintos e expondo os endpoints REST por meio do projeto CashFlow.API. Inclui ainda projetos de ***testes unitários*** (tests/Validators.Tests e tests/CommonTestUtilities) para garantir a qualidade da lógica de negócio e facilitar a criação de utilitários de teste.

## Arquitetura do Projeto

O projeto segue uma arquitetura em camadas:

```
src/
├── CashFlow.API           # Camada de apresentação (API REST)
├── CashFlow.Application   # Casos de uso / regras de negócio
├── CashFlow.Domain        # Entidades e interfaces de repositórios
├── CashFlow.Exception     # Filtros e tipos de exceção customizados
├── CashFlow.Communication # DTOs de requisição e resposta (JSON)
└── CashFlow.Infrastructure# Implementação de repositórios e DbContext (MySQL)
tests/
├── Validators.Tests       # Testes unitários de validação
└── CommonTestUtilities    # Helpers para testes
```

## Tecnologias

- **Linguagem:** C#
- **Runtime:** .NET 8.0
- **ORM:** Entity Framework Core (opcional)
- **Banco de Dados:** MySQL
- **Documentação:** Swagger / Swashbuckle
- **Testes:** xUnit / MSTest
- **Validação:** FluentValidation / Validadores customizados

## Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- **MySQL Server** (8.0 ou superior)
- Cliente HTTP (curl, Postman, Insomnia, etc.)

## Instalação e Configuração

1. Clone o repositório:

   ```bash
   git clone https://github.com/JonnyYamagushi/CashFlow.git
   cd CashFlow
   ```

2. Configure a conexão com o banco no arquivo `src/CashFlow.API/appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "Connection": "Server=localhost;Port=3306;Database=cashflow;User Id=usuario;Password=senha;"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

3. Crie o banco e a tabela manualmente (sem migrations):

   ```sql
   CREATE DATABASE cashflow;
   USE cashflow;

   CREATE TABLE Expenses (
     Id BIGINT PRIMARY KEY AUTO_INCREMENT,
     Title VARCHAR(200) NOT NULL,
     Description TEXT,
     Date DATETIME NOT NULL,
     Amount DECIMAL(18,2) NOT NULL,
     PaymentType INT NOT NULL
   );
   ```

## Executando a API

No diretório do projeto API:

```bash
cd src/CashFlow.API
dotnet restore
dotnet run
```

Por padrão, a API estará disponível em:

```
https://localhost:5001
http://localhost:5000
```

## Documentação Swagger

Acesse a documentação interativa em:

```
https://localhost:5001/swagger
```

## Endpoints da API

### Transações (Expenses)

| Rota                        | Método | Descrição                  |
| --------------------------- | ------ | -------------------------- |
| `POST /api/expenses`        | POST   | Registra nova despesa      |
| `GET /api/expenses`         | GET    | Lista todas as despesas    |
| `GET /api/expenses/{id}`    | GET    | Recupera despesa por ID    |
| `PUT /api/expenses/{id}`    | PUT    | Atualiza despesa existente |
| `DELETE /api/expenses/{id}` | DELETE | Remove despesa             |

### Relatórios (Report)

| Rota                    | Método | Parâmetro                | Descrição                                    |
| ----------------------- | ------ | ------------------------ | -------------------------------------------- |
| `GET /api/report/excel` | GET    | Header `month` (YYYY-MM) | Gera relatório Excel para o mês especificado |
| `GET /api/report/pdf`   | GET    | Query `month` (YYYY-MM)  | Gera relatório PDF para o mês especificado   |

## Exemplos de Uso

Registrar nova despesa:

```bash
curl -X POST https://localhost:5001/api/expenses \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Almoço",
    "description": "Restaurante",
    "date": "2025-07-23T12:30:00",
    "amount": 45.50,
    "paymentType": 1
}'
```

Gerar relatório Excel:

```bash
curl -X GET https://localhost:5001/api/report/excel \
  -H "month: 2025-07"
```

## Testes

Execute todos os testes unitários:

```bash
dotnet test
```

## Contribuindo

1. Faça um fork deste repositório
2. Crie uma branch para sua feature:
   ```bash
   git checkout -b feature/nome-da-feature
   ```
3. Commit suas alterações:
   ```bash
   git commit -m "Descrição da alteração"
   ```
4. Envie para o repositório remoto:
   ```bash
   git push origin feature/nome-da-feature
   ```
5. Abra um Pull Request

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE.txt). Sinta-se livre para usar, estudar, modificar e distribuir.