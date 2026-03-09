# T-00: Project Scaffolding & Repo Setup

## Status: ⬜ Not Started
## Phase: 0 — Foundation
## Dependencies: None
## Agents: backend-developer, frontend-developer, devops-engineer

---

## Objective
Initialize the monorepo with the correct directory structure, .NET solution, React project, and configuration files.

## Steps

- [ ] 1. Initialize git repo with `.gitignore` (dotnet + node + docker)
- [ ] 2. Create the directory structure defined in CLAUDE.md
- [ ] 3. **Backend:** Create .NET 8 solution with 4 projects:
  - `PhysioBook.Api` (web api project)
  - `PhysioBook.Application` (class library)
  - `PhysioBook.Domain` (class library)
  - `PhysioBook.Infrastructure` (class library)
  - Add project references following Clean Architecture dependency rules
  - Add NuGet packages: MediatR, FluentValidation, Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.AspNetCore.Authentication.JwtBearer, MassTransit.RabbitMQ, Microsoft.AspNetCore.SignalR
- [ ] 4. **Frontend:** Initialize Vite React TS project:
  - `npm create vite@latest frontend -- --template react-ts`
  - Install: tailwindcss, @tanstack/react-query, @tanstack/react-router, zustand, @dnd-kit/core, @dnd-kit/sortable, @dnd-kit/utilities, react-hook-form, @hookform/resolvers, zod, date-fns, i18next, react-i18next, @microsoft/signalr, @headlessui/react, lucide-react, clsx, tailwind-merge
  - Configure TailwindCSS with custom green color palette
  - Set up path aliases (`@/` → `src/`)
  - Create feature directory structure
- [ ] 5. Create `src/shared/` for types shared between FE and BE
- [ ] 6. Create initial `README.md` with project overview and setup instructions

## Verification
- `dotnet build` succeeds on backend solution
- `npm run build` succeeds on frontend
- All project references resolve correctly
- Directory structure matches CLAUDE.md spec

## Summary of Changes
<!-- Filled in after completion -->
