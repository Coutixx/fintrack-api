# 🏆 FinTrack API

Sistema de gestão financeira pessoal desenvolvido em **ASP.NET Core (.NET 10)** utilizando **Controllers**, **Clean Architecture (Feature-First)** e **Entity Framework Core**.

O objetivo deste projeto é desenvolver uma API completa para gerenciamento financeiro, consolidando conceitos modernos de arquitetura de software, modelagem de domínio, validações desacopladas e regras de negócio reais.

> 🚧 **Projeto em desenvolvimento**
>
> Este README funciona temporariamente como documentação técnica e roadmap oficial do projeto. Após a conclusão, será convertido para um README voltado ao uso da aplicação.

---

# 🎯 Objetivos

Durante o desenvolvimento deste projeto serão consolidados os seguintes conceitos:

- Clean Architecture
- Organização Feature-First
- Controllers
- Entity Framework Core
- PostgreSQL
- Docker (infraestrutura do banco)
- FluentValidation
- JWT Authentication
- Global Exception Handler
- Soft Delete
- Paginação e filtros
- Testes Unitários

---

# 📌 Tecnologias

- ASP.NET Core (.NET 10)
- Controllers
- Entity Framework Core
- PostgreSQL
- Docker
- FluentValidation
- JWT Bearer Authentication
- Scalar / OpenAPI
- xUnit

---

# 📂 Estrutura do Projeto

```text
FinTrack.sln
│
├── src
│   ├── FinTrack.Domain
│   │   ├── Common
│   │   └── Entities
│   │
│   ├── FinTrack.Application
│   │   ├── Common
│   │   └── Features
│   │       ├── Auth
│   │       ├── Accounts
│   │       ├── Categories
│   │       └── Transactions
│   │
│   ├── FinTrack.Infrastructure
│   │   ├── Data
│   │   ├── Security
│   │   └── Persistence
│   │
│   └── FinTrack.Api
│       ├── Controllers
│       ├── Extensions
│       ├── ExceptionHandlers
│       └── Program.cs
│
└── tests
    └── FinTrack.UnitTests
```

---

# 💾 Banco de Dados

O projeto utiliza **PostgreSQL** executado em um container Docker.

A API será executada localmente através do .NET.

Para iniciar o banco:

```bash
docker run \
--name fintrack-db \
-e POSTGRES_USER=postgres \
-e POSTGRES_PASSWORD=postgres \
-e POSTGRES_DB=fintrack \
-p 5432:5432 \
-d postgres:17
```

---

# 📋 Modelagem

## BaseEntity

| Campo | Tipo |
|--------|------|
| Id | Guid |
| CreatedAt | DateTime |
| UpdatedAt | DateTime? |
| DeletedAt | DateTime? |

---

## User

| Campo | Tipo |
|--------|------|
| Id | Guid |
| Name | string |
| Email | string |
| PasswordHash | string |

---

## Account

| Campo | Tipo |
|--------|------|
| Id | Guid |
| Name | string |
| Type | string |
| InitialBalance | decimal |
| CurrentBalance | decimal |
| UserId | Guid |

---

## Category

| Campo | Tipo |
|--------|------|
| Id | Guid |
| Name | string |
| Type | string |
| Color | string |
| UserId | Guid |

---

## Transaction

| Campo | Tipo |
|--------|------|
| Id | Guid |
| Description | string |
| Amount | decimal |
| Type | string |
| Date | DateTime |
| Status | string |
| AccountId | Guid |
| CategoryId | Guid |

---

# 🔗 Relacionamentos

```text
User
 ├── 1:N ─── Account
 └── 1:N ─── Category

Account
 └── 1:N ─── Transaction

Category
 └── 1:N ─── Transaction
```

---

# 🛡️ Regras de Negócio

## Usuários

- Cada usuário possui acesso apenas aos seus próprios dados.
- O e-mail deve ser único.
- A senha será armazenada utilizando hash.

---

## Contas

- Uma conta pertence a apenas um usuário.
- O saldo inicial não pode ser negativo.
- O saldo atual será atualizado automaticamente pelas transações.

---

## Categorias

- Cada usuário possui suas próprias categorias.
- Não é permitido criar categorias duplicadas para o mesmo usuário.

---

## Transações

- O valor deve ser maior que zero.
- Receitas aumentam o saldo da conta.
- Despesas diminuem o saldo da conta.
- Transações pendentes não alteram o saldo.
- Ao excluir uma transação paga, o saldo deve ser recalculado.
- Exclusões utilizarão Soft Delete.

---

# 🚀 Endpoints

## Auth

- POST `/api/auth/register`
- POST `/api/auth/login`

---

## Accounts

- GET `/api/accounts`
- GET `/api/accounts/{id}`
- POST `/api/accounts`
- PUT `/api/accounts/{id}`
- DELETE `/api/accounts/{id}`

---

## Categories

- GET `/api/categories`
- POST `/api/categories`
- PUT `/api/categories/{id}`
- DELETE `/api/categories/{id}`

---

## Transactions

- GET `/api/transactions`
- GET `/api/transactions/{id}`
- POST `/api/transactions`
- PUT `/api/transactions/{id}`
- DELETE `/api/transactions/{id}`

---

# 📅 Roadmap

## 🚩 Sprint 1 — Infraestrutura & Autenticação

### Objetivo

Criar a base da aplicação e implementar autenticação.

### Entregas

- [x] Criar Solution
- [x] Criar projetos da Clean Architecture
- [x] Configurar PostgreSQL
- [x] Configurar Docker
- [x] Configurar EF Core
- [x] Primeira Migration
- [x] Criar User
- [x] Cadastro
- [x] Login
- [x] JWT
- [x] Testes unitários da autenticação

### Critérios de conclusão

- Usuário consegue se cadastrar.
- Usuário consegue realizar login.
- JWT válido é emitido.
- Testes passando.

---

## 🚩 Sprint 2 — Contas

### Objetivo

Implementar o gerenciamento de contas financeiras.

### Entregas

- [x] Criar Account
- [x] FluentValidation
- [x] CRUD completo
- [x] Isolamento por usuário
- [ ] Testes unitários

### Critérios de conclusão

- CRUD funcionando.
- Soft Delete funcionando.
- Usuário acessa apenas suas contas.
- Testes passando.

---

## 🚩 Sprint 3 — Categorias

### Objetivo

Gerenciar categorias financeiras.

### Entregas

- [ ] Criar Category
- [ ] CRUD
- [ ] Impedir nomes duplicados
- [ ] Paginação
- [ ] Filtros básicos
- [ ] Testes unitários

### Critérios de conclusão

- CRUD completo.
- Validações funcionando.
- Paginação funcionando.
- Testes passando.

---

## 🚩 Sprint 4 — Transações

### Objetivo

Implementar o núcleo financeiro do sistema.

### Entregas

- [ ] Criar Transaction
- [ ] CRUD
- [ ] Atualização automática do saldo
- [ ] Estorno ao excluir
- [ ] Soft Delete
- [ ] Testes unitários

### Critérios de conclusão

- Receitas aumentam saldo.
- Despesas diminuem saldo.
- Pendentes não alteram saldo.
- Exclusão recalcula saldo.
- Testes passando.

---

## 🚩 Sprint 5 — Blindagem da API

### Objetivo

Aumentar a qualidade e a segurança da aplicação.

### Entregas

- [ ] Soft Delete
- [ ] FluentValidation em todas as funcionalidades
- [ ] Global Exception Handler
- [ ] Problem Details
- [ ] Revisão geral das regras de negócio

### Critérios de conclusão

- Todas as entradas são validadas.
- Exceções retornam respostas padronizadas.
- Nenhum endpoint possui validação manual.

---

## 🚩 Sprint 6 — Finalização

### Objetivo

Preparar o projeto para publicação.

### Entregas

- [ ] Configurar Scalar
- [ ] Revisar código
- [ ] Revisar documentação
- [ ] Limpeza geral
- [ ] Revisão final dos testes

### Critérios de conclusão

- Projeto documentado.
- Código revisado.
- Testes passando.
- API pronta para publicação.

---

# 📊 Status

**Sprint Atual**

> 🚩 Sprint 0 — Planejamento

## Progresso

- [x] Sprint 0
- [ ] Sprint 1
- [ ] Sprint 2
- [ ] Sprint 3
- [ ] Sprint 4
- [ ] Sprint 5
- [ ] Sprint 6

---

# 📖 Regra do Projeto

Antes de iniciar a próxima Sprint, a Sprint atual deve estar completamente finalizada.

Uma Sprint só é considerada concluída quando:

- ✅ Funcionalidade implementada
- ✅ Regras de negócio atendidas
- ✅ Validações implementadas
- ✅ Testes da Sprint aprovados
- ✅ Código revisado
