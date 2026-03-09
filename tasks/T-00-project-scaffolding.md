# T-00: Project Scaffolding & Repo Setup

## Status: ✅ Completed
## Phase: 0 — Foundation
## Dependencies: None
## Agents: backend-developer, frontend-developer, devops-engineer

---

## Objective
Initialize the monorepo with the correct directory structure, .NET solution, React project, and configuration files.

## Steps

- [x] 1. Initialize git repo with `.gitignore` (dotnet + node + docker)
- [x] 2. Create the directory structure defined in CLAUDE.md
- [x] 3. **Backend:** Create .NET 8 solution with 4 projects
- [x] 4. **Frontend:** Initialize Vite React TS project with all dependencies
- [x] 5. Create `src/shared/` for types shared between FE and BE
- [x] 6. Create Docker, Helm, and docs scaffolding

## Verification
- `dotnet build` succeeds on backend solution
- `npm run build` succeeds on frontend
- All project references resolve correctly
- Directory structure matches CLAUDE.md spec

## Summary of Changes
- .NET 8 solution with 4 projects (Api, Application, Domain, Infrastructure) + 2 test projects, Clean Architecture references enforced
- Vite React 18 + TypeScript + TailwindCSS v4 with all required dependencies, path aliases, feature directory structure, shared components, and lib layer (api-client, i18n with sq/en, SignalR)
- Docker: multi-stage Dockerfiles for API and frontend, docker-compose with postgres/rabbitmq
- Helm chart scaffold, docs placeholders, updated .gitignore
- Both `dotnet build` and `npm run build` pass with 0 errors
