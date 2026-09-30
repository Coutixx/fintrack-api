# FinTrack API

API REST para gerenciamento de finanças pessoais, desenvolvida com ASP.NET Core 10,
Entity Framework Core e PostgreSQL.

## Funcionalidades

- Cadastro e autenticação de usuários com JWT.
- Gerenciamento de contas financeiras.
- Gerenciamento de categorias de receitas e despesas.
- Gerenciamento de transações e atualização dos saldos das contas.
- Isolamento de contas, categorias e transações por usuário autenticado.
- Exclusão lógica de contas, categorias e transações.
- Validação de requisições com FluentValidation.
- Respostas de erro padronizadas com Problem Details.
- Documentação da API com OpenAPI e Scalar.

## Tecnologias

- .NET 10 / ASP.NET Core Controllers
- MediatR e FluentValidation
- Entity Framework Core 10
- PostgreSQL 17
- Scalar / OpenAPI
- xUnit e NSubstitute
- Docker Compose

## Arquitetura

O código é separado por responsabilidades e organizado por funcionalidade:

| Projeto | Responsabilidade |
|---------|------------------|
| `FinTrack.Domain` | Entidades, enums e tipos comuns do domínio |
| `FinTrack.Application` | Casos de uso MediatR, validações e interfaces |
| `FinTrack.Infrastructure` | Persistência EF Core, repositórios, autenticação e serviços externos |
| `FinTrack.Api` | Controllers, configuração da aplicação, OpenAPI e tratamento global de exceções |
| `FinTrack.UnitTests` | Testes unitários e testes de persistência com EF Core InMemory |

As migrations do Entity Framework ficam em
`src/FinTrack.Infrastructure/Migrations`.

Os repositórios registram as alterações no `AppDbContext`; os handlers confirmam cada
operação com `IUnitOfWork`. Assim, mudanças relacionadas — como saldo e transação — são
persistidas juntas em um único `SaveChangesAsync`.

## Configuração e execução local

### Requisitos

- .NET SDK 10
- Docker com Docker Compose
- `dotnet-ef` para criar/aplicar migrations

Instale a ferramenta do EF Core, caso ainda não esteja instalada:

```bash
dotnet tool install --global dotnet-ef
```

Copie o exemplo de variáveis de ambiente:

```bash
cp .env.example .env
```

Configure no `.env`:

- `POSTGRES_USER`, `POSTGRES_DB` e `POSTGRES_PASSWORD` para o container PostgreSQL.
- `ConnectionStrings__PostgresConnection` com os mesmos dados de conexão.
- `JwtSettings__SECRET` com uma chave secreta aleatória de pelo menos 32 bytes UTF-8.
- `JwtSettings__Issuer`, `JwtSettings__Audience` e `JwtSettings__ExpirationHours` para
  identificação do emissor, público e duração dos tokens JWT.

O arquivo `.env` é ignorado pelo Git. Não versione segredos ou credenciais.
O exemplo configura emissor `FinTrack.Api`, público `FinTrack.Client` e duração de uma
hora; personalize esses valores conforme o ambiente.

Inicie o PostgreSQL:

```bash
docker compose up -d fintrack-db
```

Aplique as migrations:

```bash
dotnet ef database update \
  --project src/FinTrack.Infrastructure/FinTrack.Infrastructure.csproj \
  --startup-project src/FinTrack.Api/FinTrack.Api.csproj
```

Execute a API:

```bash
dotnet run --project src/FinTrack.Api/FinTrack.Api.csproj --launch-profile http
```

O perfil HTTP usa `http://localhost:5266`. No GitHub Codespaces, abra/encaminhe a porta
`5266` no painel **Ports** e use a URL encaminhada.

## OpenAPI e Scalar

- Scalar: `/scalar`
- Documento OpenAPI: `/openapi/v1.json`

Por exemplo, localmente:

```text
http://localhost:5266/scalar
http://localhost:5266/openapi/v1.json
```

## Autenticação

Os endpoints de autenticação são públicos. Registre-se com `POST /api/auth/register` ou
obtenha um token com `POST /api/auth/login`. Envie o token nas rotas protegidas usando:

```text
Authorization: Bearer <token>
```

No Scalar, configure o cabeçalho `Authorization` com `Bearer ` seguido do token. Contas,
categorias e transações exigem autenticação.

## Endpoints

### Autenticação

| Método | Rota | Acesso |
|--------|------|--------|
| POST | `/api/auth/register` | Público |
| POST | `/api/auth/login` | Público |

### Contas

| Método | Rota |
|--------|------|
| GET | `/api/accounts` |
| GET | `/api/accounts/{id}` |
| POST | `/api/accounts` |
| PUT | `/api/accounts/{id}` |
| DELETE | `/api/accounts/{id}` |

### Categorias

| Método | Rota |
|--------|------|
| GET | `/api/categories` |
| GET | `/api/categories/{id}` |
| POST | `/api/categories` |
| PUT | `/api/categories/{id}` |
| DELETE | `/api/categories/{id}` |

A listagem aceita `type` opcional (`0` para receita, `1` para despesa), `page` (padrão
`1`) e `pageSize` (padrão `10`, máximo `100`). Exemplo:

```text
GET /api/categories?type=1&page=2&pageSize=5
```

A resposta inclui `categories`, `page`, `pageSize` e `totalCount`.

### Transações

| Método | Rota |
|--------|------|
| GET | `/api/accounts/transactions` |
| GET | `/api/accounts/{accountId}/transactions` |
| GET | `/api/accounts/{accountId}/transactions/{id}` |
| POST | `/api/accounts/{accountId}/transactions` |
| PUT | `/api/accounts/{accountId}/transactions/{id}` |
| DELETE | `/api/accounts/{accountId}/transactions/{id}` |

A listagem aceita `type` opcional (`0` para receita, `1` para despesa). Exemplo:

```text
GET /api/accounts/transactions?type=0
```

## Modelo de dados

As entidades herdam os campos comuns `Id`, `CreatedAt`, `UpdatedAt` e `DeletedAt`.
O soft delete é aplicado a contas, categorias e transações; usuários não são excluídos
logicamente.

| Entidade | Campos específicos | Relacionamentos |
|----------|--------------------|-----------------|
| `User` | `Name`, `Email`, `PasswordHash` | Possui contas e categorias |
| `Account` | `Name`, `Type`, `InitialBalance`, `CurrentBalance`, `UserId` | Pertence a um usuário; contém transações |
| `Category` | `Name`, `Type`, `Color`, `UserId` | Pertence a um usuário; classifica transações |
| `Transaction` | `Description`, `Amount`, `Type`, `Date`, `Status`, `AccountId`, `CategoryId` | Pertence a uma conta e categoria |

`Transaction.Date` representa somente uma data de calendário, sem horário ou fuso.
Envie e receba a data no formato ISO `yyyy-MM-dd` (por exemplo, `2026-09-30`).

### Enums

Os enums são representados por valores numéricos nas requisições:

| Enum | Valores |
|------|---------|
| `AccountType` | `0` Checking, `1` Savings, `2` Cash, `3` CreditCard |
| `TransactionType` | `0` Income, `1` Expense |
| `TransactionStatus` | `0` Pending, `1` Paid, `2` Cancelled |

## Regras de negócio

- Cada usuário acessa apenas suas próprias contas e categorias.
- O e-mail do usuário é único.
- Senhas são armazenadas como hash.
- O saldo inicial da conta não pode ser negativo; se omitido, começa em zero.
- Categorias não podem ter nomes duplicados para o mesmo usuário.
- O tipo da transação deve corresponder ao tipo de sua categoria.
- O valor da transação deve ser maior que zero.
- Transações pagas atualizam o saldo: receitas somam e despesas subtraem.
- Transações pendentes não alteram o saldo.
- Atualizar uma transação paga remove o efeito anterior e aplica o novo.
- Excluir uma transação paga estorna seu efeito no saldo.
- Exclusões de contas, categorias e transações são lógicas.
- Transações de contas excluídas logicamente não aparecem nas consultas normais.
- Uma categoria excluída não apaga o histórico: transações existentes continuam listadas
  e podem ser atualizadas usando o tipo registrado na categoria original.

## Tratamento de erros

Erros são convertidos em respostas Problem Details. Erros de validação retornam
`ValidationProblemDetails`, com mensagens associadas às propriedades inválidas.

| Condição | HTTP |
|----------|------|
| Validação ou argumento inválido | 400 |
| Credenciais inválidas | 401 |
| Entidade não encontrada | 404 |
| Conflito de persistência | 409 |
| Erro inesperado | 500 |

## Testes e build

Execute na raiz do repositório:

```bash
dotnet test
dotnet build
```

Os testes EF Core InMemory verificam os filtros de exclusão lógica e a persistência
conjunta de saldo e transação. Eles não substituem a aplicação da migration no PostgreSQL;
aplique-a ao banco local com `dotnet ef database update` após atualizar o projeto.

## Convenção de commits

O histórico do projeto usa mensagens no formato Conventional Commits, com o tipo,
uma área opcional e uma descrição da alteração. Exemplos alinhados às alterações do
projeto:

```text
feat(transactions): implement transaction balance updates
fix(categories): allow keeping the category name on update
test(api): cover global exception handler
docs(api): document local setup and endpoints
```
