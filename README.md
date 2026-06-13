# TaskFlow

App de gestão de tarefas full-stack — projeto pessoal / PDI.

## Stack

- **Backend:** .NET 8, Clean Architecture, EF Core, PostgreSQL
- **Frontend:** Angular 21, Angular Material
- **Infra:** Docker, Docker Compose, GitHub Actions

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

## Rodar localmente

```bash
# 1. Clone o repositório
git clone <url-do-repo>
cd taskflow

# 2. Configure as variáveis de ambiente
cp .env.example .env
# Edite .env e preencha JWT_KEY e POSTGRES_PASSWORD

# 3. Suba os containers
docker compose up --build -d
```

| Serviço  | URL                           |
|----------|-------------------------------|
| Frontend | http://localhost:4200         |
| API      | http://localhost:8080/swagger |
| Postgres | localhost:5432                |

## Parar os containers

```bash
docker compose down
```

## CI — GitHub Actions

O pipeline roda automaticamente em todo push e pull request para `main`.

| Job      | O que faz                                                      |
|----------|----------------------------------------------------------------|
| backend  | `dotnet restore` → `dotnet build` → `dotnet test` (45 testes) |
| frontend | `npm ci` → `ng build`                                         |

## Documentação

- [Arquitetura do sistema](docs/architecture.md) — camadas, fluxo de requisição, domínio, API, frontend
- [Decisões técnicas](docs/decisions.md) — ADRs com motivo e consequência de cada escolha

## Estrutura do projeto

```
taskflow/
├── backend/
│   ├── TaskFlow.Api/            # Controllers, Contracts, Middlewares
│   ├── TaskFlow.Application/    # Use Cases
│   ├── TaskFlow.Domain/         # Entidades, Enums, Exceptions
│   ├── TaskFlow.Infrastructure/ # EF Core, AppDbContext
│   ├── TaskFlow.Tests/          # xUnit, EF InMemory
│   └── Dockerfile
├── frontend/
│   └── taskflow-web/            # Angular standalone components
│       └── Dockerfile
├── docker-compose.yml
└── .env.example
```
