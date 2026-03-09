# PhysioBook — Clinic Management SaaS

## Project Overview

PhysioBook is a multi-tenant SaaS platform for physiotherapy clinics in Albania. It replaces paper notebook scheduling with a drag-and-drop calendar, patient history management, walk-in queue, and online booking with payment integration.

**Owner:** Switch Technologies, Elbasan, Albania
**Stack:** .NET 8 Web API · PostgreSQL 16 · Vite + React 18 + TypeScript + TailwindCSS · RabbitMQ · Docker + Helm + K3s
**Deployment:** Self-hosted in Elbasan

---

## Architecture Rules

### Backend (.NET 8 Web API)
- Solution structure: `src/PhysioBook.Api`, `src/PhysioBook.Domain`, `src/PhysioBook.Infrastructure`, `src/PhysioBook.Application`
- Follow Clean Architecture: Domain has zero dependencies, Application depends on Domain, Infrastructure implements interfaces from Application
- Use MediatR for CQRS pattern (Commands and Queries)
- Use FluentValidation for all request validation
- Use Entity Framework Core 8 with PostgreSQL (Npgsql provider)
- All endpoints must be versioned under `/api/v1/`
- JWT authentication with refresh token rotation (access: 15min, refresh: 7 days)
- Every entity must include `tenant_id` — NO EXCEPTIONS
- Use PostgreSQL Row-Level Security (RLS) for tenant isolation
- Set `app.current_tenant` session variable in middleware before any DB operation
- All IDs are UUIDs (`Guid` in C#)
- Use SignalR for real-time calendar updates
- Use MassTransit with RabbitMQ for async messaging (SMS, email, payments, reports)
- Never use `DateTime` — always `DateTimeOffset` with IANA timezone support
- Password hashing: bcrypt with cost factor 12
- All responses follow a consistent envelope: `{ data, errors, meta }`

### Frontend (Vite + React 18 + TypeScript + TailwindCSS)
- Feature-based module structure: `src/features/{calendar,patients,walkins,booking,analytics}`
- Each feature folder: `components/`, `hooks/`, `api/`, `types/`, `utils/`
- Use `@dnd-kit/core` + `@dnd-kit/sortable` for drag-and-drop calendar
- Use TanStack Query (React Query) for all server state — no Redux for server data
- Use Zustand for client-only UI state (filters, preferences, sidebar state)
- Use TanStack Router for type-safe routing
- Use `react-hook-form` + `zod` for all forms
- Use `date-fns` with Albanian locale (`sq`) for date formatting
- SignalR client (`@microsoft/signalr`) for real-time calendar sync
- All API calls go through a typed `apiClient` wrapper with automatic token refresh
- TailwindCSS only — no CSS modules, no styled-components
- Responsive design: mobile-first, must work on tablets (receptionists use tablets)
- All text must support Albanian (sq) and English (en) via i18next

### Database (PostgreSQL 16)
- All tables have: `id` (UUID PK), `tenant_id` (UUID FK NOT NULL), `created_at`, `updated_at`
- Enable `pg_trgm` extension for fuzzy search
- Use JSONB for flexible settings columns
- Naming: snake_case for all tables and columns
- Foreign keys must have `ON DELETE` behavior explicitly defined
- Indexes: always index `(tenant_id, <most_filtered_column>)`
- Use partial indexes for status-filtered queries

### Docker & Deployment
- Every service gets its own Dockerfile with multi-stage build
- Docker Compose for local development
- Helm charts in `deploy/helm/` for K3s production
- Nginx Ingress Controller + Let's Encrypt for SSL
- Environment config via Kubernetes Secrets — never hardcode secrets

---

## Project Structure

```
physiobook/
├── .claude/
│   ├── agents/               # Claude Code subagents
│   └── commands/             # Custom slash commands
├── src/
│   ├── backend/
│   │   ├── PhysioBook.sln
│   │   ├── src/
│   │   │   ├── PhysioBook.Api/           # Controllers, middleware, SignalR hubs
│   │   │   ├── PhysioBook.Application/   # Commands, Queries, DTOs, Interfaces
│   │   │   ├── PhysioBook.Domain/        # Entities, Value Objects, Enums
│   │   │   └── PhysioBook.Infrastructure/# EF Core, Repos, RabbitMQ, Services
│   │   └── tests/
│   │       ├── PhysioBook.UnitTests/
│   │       └── PhysioBook.IntegrationTests/
│   ├── frontend/
│   │   ├── package.json
│   │   ├── vite.config.ts
│   │   ├── tailwind.config.ts
│   │   ├── tsconfig.json
│   │   └── src/
│   │       ├── app/                      # App shell, router, providers
│   │       ├── features/
│   │       │   ├── auth/
│   │       │   ├── calendar/
│   │       │   ├── patients/
│   │       │   ├── walkins/
│   │       │   ├── booking/
│   │       │   └── analytics/
│   │       ├── shared/                   # Shared components, hooks, utils
│   │       └── lib/                      # API client, i18n, signalr
│   └── shared/                           # Shared types between FE and BE
├── db/
│   ├── migrations/
│   └── seeds/
├── deploy/
│   ├── docker/
│   │   ├── docker-compose.yml
│   │   ├── Dockerfile.api
│   │   └── Dockerfile.frontend
│   └── helm/
│       └── physiobook/
├── docs/
│   ├── ARCHITECTURE.md
│   ├── API.md
│   └── DATABASE.md
├── tasks/                                # Task files for Claude Code workflow
├── ROADMAP.md
└── CLAUDE.md                             # This file
```

---

## Verification & Definition of Done

Before marking ANY task as complete, run these checks:

**Backend:**
- `dotnet build` — no errors, no warnings
- `dotnet test` — all tests pass
- New endpoints have integration tests
- All new entities have `tenant_id` with RLS policy

**Frontend:**
- `npm run build` — no TypeScript errors
- `npm run lint` — no ESLint errors
- Components render correctly on 768px (tablet) and 1440px (desktop)
- Forms validate with zod schemas

**Database:**
- Migrations run cleanly: `dotnet ef database update`
- RLS policies tested: switching tenant context must hide other tenant data

---

## Sub-Agent Routing Rules

**Parallel dispatch** (ALL conditions must be met):
- 3+ unrelated tasks or independent domains
- No shared state between tasks
- Clear file boundaries with no overlap (e.g., backend agent + frontend agent + migration agent)

**Sequential dispatch** (ANY condition triggers):
- Tasks have dependencies (e.g., API must exist before frontend can call it)
- Shared files or state (merge conflict risk)
- Unclear scope (need to understand before proceeding)

**Domain Parallel Patterns:**
When implementing features across domains, spawn parallel agents:
- **Backend agent**: Controllers, services, MediatR handlers, EF configurations
- **Frontend agent**: React components, hooks, API calls, TailwindCSS
- **Database agent**: Migrations, seeds, RLS policies, indexes
- **Test agent**: Unit tests, integration tests

Each agent owns their domain. No file overlap.

---

## Conventions

- Git commits: `feat:`, `fix:`, `chore:`, `refactor:`, `test:`, `docs:` prefixes
- Branch naming: `feature/TASK-XX-short-description`, `fix/TASK-XX-short-description`
- PR titles match the task title
- All PRs require passing CI before merge
- One task = one branch = one PR
