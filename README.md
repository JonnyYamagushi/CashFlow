# CashFlow

## Sumário

- [Descrição](#descrição)
- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Configuração](#instalação-e-configuração)
- [Executando a API](#executando-a-api)
- [Documentação Swagger](#documentação-swagger)
- [Autenticação](#autenticação)
- [Endpoints da API](#endpoints-da-api)
  - [Usuários (User)](#usuários-user)
  - [Login](#login)
  - [Transações (Expenses)](#transações-expenses)
  - [Relatórios (Report)](#relatórios-report)
- [Exemplos de Uso](#exemplos-de-uso)
- [Testes](#testes)
- [Contribuindo](#contribuindo)
- [Licença](#licença)

## Descrição

CashFlow é uma API desenvolvida em **C# (.NET 8.0)** como parte da formação em .NET da **[Rocketseat](https://www.rocketseat.com.br/)**. Seu objetivo é fornecer uma base sólida para o gerenciamento de despesas, permitindo o cadastro de usuários, autenticação via JWT, o registro e a consulta de transações financeiras, bem como a ***geração de relatórios em Excel e PDF***.

A solução adota **Clean Architecture / Domain-Driven Design (DDD)**, separando claramente as responsabilidades em projetos distintos e expondo os endpoints REST por meio do projeto `CashFlow.API`. Inclui uma suíte de testes robusta — testes unitários de casos de uso e validadores, testes de integração da Web API e uma biblioteca de utilitários de teste (builders).

## Arquitetura do Projeto

O projeto segue uma arquitetura em camadas com dependências apontando para o domínio:

```
src/
├── CashFlow.API            # Apresentação (REST): Controllers, Filters, Middleware, Token
├── CashFlow.Application     # Casos de uso (UseCases), AutoMapper e validação (FluentValidation)
├── CashFlow.Domain          # Entidades, interfaces de repositórios, Security e Services
├── CashFlow.Communication   # DTOs de Requests/Responses e Enums (contrato JSON)
├── CashFlow.Exception       # Exceções customizadas + mensagens em resource
└── CashFlow.Infrastructure  # EF Core (DbContext), UnitOfWork, repositórios, JWT, BCrypt, DI
tests/
├── UseCases.Test            # Testes unitários dos casos de uso
├── Validators.Tests         # Testes unitários dos validadores
├── WebApi.Test              # Testes de integração da Web API
└── CommonTestUtilities      # Builders/helpers reutilizáveis para os testes
```

Padrões adotados:

- **Controllers finos**: injetam o caso de uso via `[FromServices]` e não contêm regra de negócio; cada ação declara os `[ProducesResponseType]` por status. O `[Route]` fica separado do verbo HTTP.
- **Validação no caso de uso** (não no controller), com FluentValidation.
- **Repositórios segregados** por intenção: `ReadOnly`, `WriteOnly` e `UpdateOnly`, coordenados por `IUnitOfWork`.
- **Tratamento de erros centralizado** via `ExceptionFilter` e hierarquia própria de exceções (`CashFlowException`).

## Tecnologias

- **Linguagem:** C#
- **Runtime:** .NET 8.0
- **ORM:** Entity Framework Core 8 (provider **Pomelo** para MySQL)
- **Banco de Dados:** MySQL 8.0+
- **Autenticação:** JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer` + `System.IdentityModel.Tokens.Jwt`)
- **Hash de senha:** BCrypt (`BCrypt.Net-Next`)
- **Mapeamento:** AutoMapper
- **Validação:** FluentValidation
- **Relatórios:** ClosedXML (Excel) e PDFsharp-MigraDoc (PDF)
- **Documentação:** Swagger / Swashbuckle
- **Testes:** xUnit

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

2. Configure a **connection string** e a **chave de assinatura do JWT**. Os valores padrão em `src/CashFlow.API/appsettings.json` são apenas placeholders (`{SECRET}`) — **não coloque segredos reais no arquivo versionado**. Prefira [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) em desenvolvimento:

   ```bash
   cd src/CashFlow.API
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:Connection" "Server=localhost;Port=3306;Database=cashflow;Uid=usuario;Pwd=senha;"
   dotnet user-secrets set "Settings:Jwt:SigningKey" "uma-chave-secreta-longa-e-aleatoria"
   ```

   Estrutura de configuração esperada (`appsettings.json`):

   ```json
   {
     "ConnectionStrings": {
       "Connection": "{SECRET}"
     },
     "Settings": {
       "Jwt": {
         "SigningKey": "{SECRET}",
         "ExpiresMinutes": 1000
       }
     }
   }
   ```

3. **Banco de dados**: o schema é gerenciado por **migrations do EF Core** — não é necessário criar tabelas manualmente. A API executa `Database.MigrateAsync()` automaticamente na inicialização (exceto no ambiente de testes), criando/atualizando o banco. Basta que o servidor MySQL esteja acessível e o banco (`cashflow`) exista ou possa ser criado pelo usuário informado.

   Para aplicar as migrations manualmente, se preferir:

   ```bash
   dotnet ef database update --project src/CashFlow.Infrastructure --startup-project src/CashFlow.API
   ```

## Executando a API

No diretório do projeto API:

```bash
cd src/CashFlow.API
dotnet restore
dotnet run
```

Por padrão (perfil de desenvolvimento), a API fica disponível em:

```
https://localhost:7133
http://localhost:5029
```

## Documentação Swagger

Em ambiente de desenvolvimento, a documentação interativa abre automaticamente em `/swagger`:

```
https://localhost:7133/swagger
```

O Swagger está configurado com o esquema **Bearer**, permitindo autenticar as requisições diretamente pela interface (botão **Authorize**).

## Autenticação

A API usa **JWT Bearer**. O fluxo típico é:

1. Cadastrar um usuário em `POST /api/user`.
2. Autenticar em `POST /api/login` e obter o token.
3. Enviar o token no header `Authorization: Bearer <token>` nas rotas protegidas.

A maioria dos endpoints exige autenticação. Os relatórios (`/api/report/*`) exigem, além do token, o papel (role) **ADMIN**.

## Endpoints da API

> As rotas protegidas exigem o header `Authorization: Bearer <token>`.

### Usuários (User)

| Rota                            | Método | Auth  | Descrição                          |
| ------------------------------- | ------ | ----- | ---------------------------------- |
| `POST /api/user`                | POST   | —     | Cadastra um novo usuário           |
| `GET /api/user`                 | GET    | JWT   | Retorna o perfil do usuário logado |
| `PUT /api/user`                 | PUT    | JWT   | Atualiza o perfil do usuário       |
| `PUT /api/user/change-password` | PUT    | JWT   | Altera a senha do usuário          |
| `DELETE /api/user`              | DELETE | JWT   | Remove a conta do usuário logado   |

### Login

| Rota              | Método | Auth | Descrição                                |
| ----------------- | ------ | ---- | ---------------------------------------- |
| `POST /api/login` | POST   | —    | Autentica e retorna o token JWT de acesso |

### Transações (Expenses)

| Rota                        | Método | Auth | Descrição                  |
| --------------------------- | ------ | ---- | -------------------------- |
| `POST /api/expenses`        | POST   | JWT  | Registra nova despesa      |
| `GET /api/expenses`         | GET    | JWT  | Lista as despesas          |
| `GET /api/expenses/{id}`    | GET    | JWT  | Recupera despesa por ID    |
| `PUT /api/expenses/{id}`    | PUT    | JWT  | Atualiza despesa existente |
| `DELETE /api/expenses/{id}` | DELETE | JWT  | Remove despesa             |

### Relatórios (Report)

Exigem o papel **ADMIN**. O mês é informado via query string `month` (uma data — o mês/ano é o que importa). Retornam `204 No Content` quando não há despesas no período.

| Rota                    | Método | Auth        | Parâmetro                     | Descrição                                    |
| ----------------------- | ------ | ----------- | ----------------------------- | -------------------------------------------- |
| `GET /api/report/excel` | GET    | JWT (ADMIN) | Query `month` (ex.: 2025-07-01) | Gera relatório Excel (`.xlsx`) do mês         |
| `GET /api/report/pdf`   | GET    | JWT (ADMIN) | Query `month` (ex.: 2025-07-01) | Gera relatório PDF do mês                     |

## Exemplos de Uso

Autenticar e obter o token:

```bash
curl -X POST https://localhost:7133/api/login \
  -H "Content-Type: application/json" \
  -d '{ "email": "usuario@email.com", "password": "SenhaForte123!" }'
```

Registrar nova despesa (rota protegida):

```bash
curl -X POST https://localhost:7133/api/expenses \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <SEU_TOKEN>" \
  -d '{
    "title": "Almoço",
    "description": "Restaurante",
    "date": "2025-07-23T12:30:00",
    "amount": 45.50,
    "paymentType": 1
}'
```

Gerar relatório Excel (requer role ADMIN):

```bash
curl -X GET "https://localhost:7133/api/report/excel?month=2025-07-01" \
  -H "Authorization: Bearer <SEU_TOKEN>" \
  --output report.xlsx
```

## Testes

Execute toda a suíte (unitários + integração):

```bash
dotnet test
```

A pasta `tests/` contém:

- **UseCases.Test** — testes unitários dos casos de uso (Expenses, Users, Login, Reports).
- **Validators.Tests** — testes dos validadores (FluentValidation).
- **WebApi.Test** — testes de integração ponta a ponta da API.
- **CommonTestUtilities** — builders/helpers reutilizáveis (entidades, requests, repositórios mockados, mapper, token e usuário logado).

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
