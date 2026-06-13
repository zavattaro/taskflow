# Arquitetura do TaskFlow

## Visão Geral

O TaskFlow é uma aplicação full-stack de gestão de tarefas composta por três serviços independentes orquestrados via Docker Compose:

```
┌─────────────────────────────────────────────────────┐
│                   Docker Compose                    │
│                                                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────┐  │
│  │  taskflow-   │  │  taskflow-   │  │ postgres │  │
│  │     web      │  │     api      │  │   :5432  │  │
│  │  nginx:80    │  │  .NET 8:8080 │  │          │  │
│  └──────┬───────┘  └──────┬───────┘  └────┬─────┘  │
│         │                 │               │         │
│         │  HTTP (browser) │    EF Core    │         │
│         └─────────────────┘               │         │
│                           └───────────────┘         │
└─────────────────────────────────────────────────────┘

Portas expostas ao host:
  :4200 → frontend
  :8080 → API
  :5432 → PostgreSQL
```

---

## Backend — Clean Architecture

O backend segue Clean Architecture com separação estrita de responsabilidades em quatro projetos:

```
TaskFlow.Domain
  └── Entidades, Enums, Exceptions
      Regras de negócio puras — sem dependências externas

TaskFlow.Application
  └── Use Cases (Command/Query + Result)
      Orquestra o domínio — sem acesso direto ao banco

TaskFlow.Infrastructure
  └── AppDbContext, Migrations
      Implementação concreta do acesso a dados (EF Core + PostgreSQL)

TaskFlow.Api
  └── Controllers, Contracts, Middlewares
      Camada HTTP — recebe, valida entrada, delega ao use case, retorna resposta

TaskFlow.Tests
  └── xUnit + EF InMemory
      Testes unitários e de integração dos use cases e entidades
```

### Fluxo de uma requisição

```
Request HTTP
    ↓
Controller (valida input básico, extrai userId do JWT)
    ↓
Command/Query (DTO de entrada tipado)
    ↓
Use Case (lógica de aplicação, regras de negócio)
    ↓
Domain Entity (validações via factory Create())
    ↓
AppDbContext (EF Core → PostgreSQL)
    ↓
Result (DTO de saída)
    ↓
Response HTTP
```

Exceções de domínio (`ValidationException`, `NotFoundException`, `AccessDeniedException`, `AuthenticationException`) são capturadas pelo `ExceptionHandlingMiddleware` e transformadas em respostas HTTP tipadas (`ApiErrorResponse`).

---

## Domínio

### Entidades

**User**
- `Id` (Guid), `Name`, `Email`, `PasswordHash`
- Relacionamento: 1 usuário → N projetos
- Factory: `User.Create(name, email, passwordHash)`

**Project**
- `Id` (Guid), `Name`, `Description?`, `UserId`
- Relacionamento: 1 projeto → N tarefas
- Factory: `Project.Create(userId, name, description)`

**TaskItem**
- `Id` (Guid), `Title`, `Description?`, `Status`, `ProjectId`
- Status: `Todo (1)` → `Doing (2)` → `Done (3)`
- Factory: `TaskItem.Create(projectId, title, description)`

### Padrões aplicados nas entidades
- Private setters — estado só muda via métodos explícitos
- Construtor protegido — EF Core instancia internamente
- Factory method `Create()` — única entrada de criação, com validações
- Domain exceptions lançadas dentro do domínio

---

## API — Endpoints

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/api/users/register` | ❌ | Cadastra novo usuário |
| POST | `/api/users/login` | ❌ | Autentica e retorna JWT |
| POST | `/api/projects` | ✅ | Cria projeto |
| GET | `/api/projects` | ✅ | Lista projetos do usuário |
| POST | `/api/projects/{id}/tasks` | ✅ | Cria tarefa no projeto |
| GET | `/api/projects/{id}/tasks` | ✅ | Lista tarefas do projeto |
| PATCH | `/api/projects/{id}/tasks/{taskId}/status` | ✅ | Atualiza status da tarefa |

Autenticação via JWT Bearer. O token é emitido no login com expiração configurável via `Jwt:ExpirationInHours`.

Ownership é verificado nos use cases — um usuário só acessa seus próprios projetos e tarefas.

---

## Frontend — Angular

Estrutura de pastas:

```
src/app/
├── core/
│   ├── guards/       → AuthGuard (protege rotas privadas)
│   ├── interceptors/ → AuthInterceptor (injeta Bearer token)
│   └── services/     → AuthService, ProjectsService, TasksService
├── features/
│   ├── auth/         → LoginComponent
│   ├── projects/     → ProjectsComponent
│   ├── tasks/        → TasksComponent
│   └── home.component.ts
└── shared/
    └── components/   → AppButton, AppHeader, AppLoading,
                        AppEmptyState, ErrorMessage, SuccessMessage
```

- Standalone components (sem NgModules)
- Angular Material como biblioteca de UI (tema roxo `#8c00ba`)
- Rotas protegidas via `AuthGuard`
- Token JWT armazenado e injetado automaticamente via `AuthInterceptor`

---

## Infraestrutura

### Variáveis de Ambiente

Todas as configurações sensíveis são injetadas via `.env` (não versionado):

| Variável | Usado por |
|----------|-----------|
| `POSTGRES_DB/USER/PASSWORD` | postgres, api |
| `JWT_KEY/ISSUER/AUDIENCE` | api |
| `CONNECTION_STRING` | api |
| `API_BASE_URL` | build do frontend |

### CI — GitHub Actions

Pipeline em `.github/workflows/ci.yml`, disparado em push/PR para `main`:

- **Job backend:** restore → build → test (45 testes)
- **Job frontend:** npm ci → ng build
