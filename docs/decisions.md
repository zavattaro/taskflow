# Decisões Técnicas

Registro das principais decisões de arquitetura e design adotadas no projeto.

---

## ADR-001 — Clean Architecture no backend

**Decisão:** Organizar o backend em quatro projetos separados seguindo Clean Architecture (Domain → Application → Infrastructure → Api).

**Motivo:** Separação clara de responsabilidades. O domínio não conhece infraestrutura; os use cases não conhecem HTTP. Facilita testes unitários — os use cases são testados com EF InMemory sem precisar de banco real.

**Consequência:** Mais arquivos e projetos do que uma estrutura monolítica, mas cada camada tem uma única responsabilidade e pode ser testada isoladamente.

---

## ADR-002 — Factory method nas entidades de domínio

**Decisão:** Entidades possuem construtor `protected` (para EF Core) e factory method público estático `Create()` como único ponto de criação.

**Motivo:** Centraliza validações de invariantes de domínio. Não é possível criar uma entidade em estado inválido — `User.Create("", email, hash)` lança `ValidationException` antes de instanciar.

**Consequência:** Construtores públicos ficam impossibilitados de uso externo. `Create()` é sempre o ponto de entrada, tornando o código autodocumentado.

---

## ADR-003 — Domain Exceptions tipadas

**Decisão:** Criar exceções específicas para cada categoria de erro de domínio: `ValidationException`, `NotFoundException`, `AccessDeniedException`, `AuthenticationException`.

**Motivo:** O `ExceptionHandlingMiddleware` captura cada tipo e retorna o HTTP status correto automaticamente (400, 404, 403, 401). Controllers não precisam de try/catch — lançam ou delegam ao use case que lança.

**Consequência:** Fluxo de erro previsível e consistente em toda a API sem código repetitivo nos controllers.

---

## ADR-004 — Records para contratos da API

**Decisão:** Usar C# `record` para todos os DTOs de entrada (Commands, Queries) e saída (Results, Responses).

**Motivo:** Records são imutáveis por padrão, têm igualdade por valor e sintaxe concisa. Contratos de API não devem ser mutáveis após criados.

**Consequência:** Nenhum setter nos DTOs — dados trafegam de forma previsível sem risco de mutação acidental.

---

## ADR-005 — CancellationToken em todos os use cases

**Decisão:** Todos os use cases recebem `CancellationToken` e o repassam para as queries do EF Core.

**Motivo:** Permite que requisições canceladas (cliente desconectou, timeout) não continuem consumindo recursos no banco de dados.

**Consequência:** Contratos de use case ficam levemente mais verbosos, mas o comportamento sob carga é mais resiliente.

---

## ADR-006 — Verificação de ownership nos use cases

**Decisão:** A verificação de que o usuário autenticado é dono do recurso (projeto, tarefa) é feita dentro do use case, não no controller.

**Motivo:** Regra de negócio — quem pode acessar o quê. Controllers são responsáveis apenas por HTTP; use cases são responsáveis pelas regras de aplicação.

**Consequência:** Controllers ficam magros. A segurança de acesso é testada nos testes de use case, não em testes de integração HTTP.

---

## ADR-007 — Standalone Components no Angular

**Decisão:** Usar Angular standalone components (sem NgModules).

**Motivo:** Padrão recomendado a partir do Angular 17+. Cada componente declara suas próprias dependências via `imports`, tornando o acoplamento explícito e a árvore de dependências mais fácil de rastrear.

**Consequência:** Sem `AppModule`. Cada componente é autossuficiente e pode ser reutilizado ou testado sem contexto de módulo.

---

## ADR-008 — Docker multi-stage build

**Decisão:** Dockerfiles com multi-stage: stage de build separado do stage de runtime.

**Motivo:** A imagem final não carrega SDK do .NET (700MB+) nem node_modules (300MB+). Backend usa apenas `aspnet:8.0` runtime; frontend usa apenas `nginx:alpine`.

**Consequência:** Imagens menores e mais seguras em produção. O processo de build fica isolado e reproduzível independente do ambiente local.

---

## ADR-009 — Variáveis de ambiente via .env + Docker Compose

**Decisão:** Secrets (JWT key, senha do banco) nunca são commitados. Ficam em `.env` (no .gitignore) e são injetados via Docker Compose.

**Motivo:** Segurança básica — credentials fora do repositório. `.env.example` documenta o que precisa ser configurado.

**Consequência:** Novo colaborador precisa criar `.env` a partir do `.env.example`. Nenhuma credencial real no histórico do git.

---

## ADR-010 — PostgreSQL como banco de dados

**Decisão:** PostgreSQL 17 via Docker para desenvolvimento local.

**Motivo:** Open source, robusto, suportado nativamente pelo EF Core via Npgsql. Consistência entre ambiente local (Docker) e eventual deploy em produção.

**Consequência:** EF Core com Npgsql como provider. Migrations versionadas no repositório.
