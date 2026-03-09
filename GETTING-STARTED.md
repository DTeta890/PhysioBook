# PhysioBook — Claude Code Getting Started Guide

## Prerequisites

Before you begin, make sure you have:

- **Claude Code CLI** installed: `npm install -g @anthropic-ai/claude-code`
- **Git** configured with your GitHub account
- **Docker Desktop** or Docker Engine + Docker Compose
- **.NET 8 SDK** installed
- **Node.js 20+** and npm
- A terminal that supports multiple sessions (iTerm2, Windows Terminal, tmux, etc.)

---

## Step 1: Create Your Repository

```bash
# Create the repo on GitHub first, then:
mkdir physiobook && cd physiobook
git init
```

## Step 2: Copy the Scaffold Files

Copy the entire scaffold into your repo root:
```
physiobook/
├── .claude/
│   ├── agents/
│   │   ├── backend-developer.md
│   │   ├── frontend-developer.md
│   │   ├── database-engineer.md
│   │   ├── test-engineer.md
│   │   └── devops-engineer.md
│   └── commands/
│       ├── next-task.md
│       ├── implement.md
│       ├── status.md
│       └── parallel-build.md
├── tasks/                          # All 49 task files
├── docs/
├── CLAUDE.md                       # The brain — Claude reads this first
├── ROADMAP.md                      # Task tracker with dependencies
└── GETTING-STARTED.md              # This file
```

## Step 3: Initial Commit

```bash
git add .
git commit -m "chore: initial project scaffold with Claude Code workflow"
git remote add origin git@github.com:your-org/physiobook.git
git push -u origin main
```

## Step 4: Add .gitignore

Add `.claude/worktrees/` to your `.gitignore` so worktree directories don't pollute your repo.

---

## How to Work with Claude Code

### Starting a Session

```bash
cd physiobook
claude
```

Claude will automatically read `CLAUDE.md` and understand the entire project.

### Your Core Commands

| Command | What It Does |
|---------|-------------|
| `/project:next-task` | Finds the next available task (dependencies met) and starts it |
| `/project:implement T-05` | Implements a specific task by ID |
| `/project:status` | Shows project progress, blockers, and next steps |
| `/project:parallel-build T-10,T-11,T-13` | Runs multiple tasks in parallel worktrees |

### Working on Your First Task

The easiest way to start:

```bash
claude
> /project:next-task
```

Claude will pick T-00 (project scaffolding), show you a plan, and ask for confirmation. Say "yes" and it will build the entire project structure.

### Sequential Workflow (One Task at a Time)

This is the simplest approach — great when starting out:

```bash
claude
> /project:implement T-00
# Wait for completion...
> /project:implement T-01
# Wait for completion...
> /project:status
# See what's available next
```

### Parallel Workflow (Multiple Tasks at Once)

Once you're comfortable, use worktrees to run tasks in parallel:

```bash
# Terminal 1: Backend work
claude --worktree feature-T-02-database
> Read tasks/T-02-database-schema.md and implement it. You are the database-engineer agent.

# Terminal 2: Frontend work (independent of T-02)
claude --worktree feature-T-06-frontend
> Read tasks/T-06-frontend-scaffold.md and implement it. You are the frontend-developer agent.

# Terminal 3: Docker work (independent of T-02 and T-06)
claude --worktree feature-T-01-docker
> Read tasks/T-01-docker-compose.md and implement it. You are the devops-engineer agent.
```

All three run simultaneously. When done, merge each branch:

```bash
git checkout main
git merge feature/T-01-docker --no-ff
git merge feature/T-02-database --no-ff
git merge feature/T-06-frontend --no-ff
```

### Using Subagents (Claude Dispatches Agents Itself)

For a fully autonomous experience, let Claude dispatch subagents:

```bash
claude
> Implement T-10 (Therapist CRUD) as a full-stack feature. 
> Dispatch the backend-developer agent for API + the frontend-developer agent for UI.
> Use worktree isolation for each agent.
```

Claude will spawn two background agents, each in their own worktree, working in parallel.

---

## Recommended Task Order (Phase 0)

These are the **exact prompts** to give Claude Code for Phase 0:

### Prompt 1 — Project Scaffolding (T-00)
```
Read CLAUDE.md and tasks/T-00-project-scaffolding.md. 
Set up the complete project: .NET 8 solution with Clean Architecture 
(Api, Application, Domain, Infrastructure projects) and Vite React TS 
frontend with TailwindCSS. Follow the exact project structure from 
CLAUDE.md. Install all specified NuGet packages and npm packages.
Verify both `dotnet build` and `npm run build` pass.
Mark T-00 as complete in ROADMAP.md.
```

### Prompt 2 — Docker Compose (T-01)
```
Read tasks/T-01-docker-compose.md. Create the Docker Compose local dev 
environment with PostgreSQL 16 (with uuid-ossp and pg_trgm), Redis, 
RabbitMQ, MinIO, and the API/Frontend services. Include health checks, 
.env.example, and a Makefile for convenience commands. Verify all 
services start with `docker-compose up -d`.
Mark T-01 as complete in ROADMAP.md.
```

### Prompt 3a — Database (T-02) — CAN RUN PARALLEL WITH 3b
```
Read tasks/T-02-database-schema.md. Create the PostgreSQL foundation: 
tenants table, users table, refresh_tokens table. Enable RLS on users 
with tenant isolation policy. Create the updated_at trigger function. 
Add all specified indexes. Create seed data with a demo tenant and 
admin user. Write EF Core entity configurations. Test RLS by verifying 
cross-tenant queries return empty.
Mark T-02 as complete in ROADMAP.md.
```

### Prompt 3b — Backend Scaffold (T-03) — CAN RUN PARALLEL WITH 3a
```
Read tasks/T-03-backend-scaffold.md. Set up the .NET 8 Clean 
Architecture solution structure. Configure EF Core with PostgreSQL, 
add MediatR with pipeline behaviors (validation, logging), configure 
JWT authentication in Program.cs, set up the API response envelope 
pattern, add global exception handling middleware, and create the 
health check endpoint. Verify `dotnet build` passes.
Mark T-03 as complete in ROADMAP.md.
```

### Prompt 4 — Multi-Tenancy (T-04)
```
Read tasks/T-04-multitenancy.md. Implement the multi-tenant middleware 
that extracts tenant_id from JWT claims and sets the PostgreSQL session 
variable `app.current_tenant`. Configure EF Core global query filters 
for tenant_id on all entities. Create the TenantProvider service. Write 
integration tests proving one tenant cannot see another tenant's data.
Mark T-04 as complete in ROADMAP.md.
```

### Prompt 5 — Auth Backend (T-05)
```
Read tasks/T-05-auth-backend.md. Implement the complete JWT auth 
system: login, register, refresh token rotation, logout, get current 
user, change password. Use bcrypt for passwords, generate JWT with 
tenant_id claim, store hashed refresh tokens. Write unit tests for all 
handlers and integration tests for the full auth flow including 
multi-tenant isolation.
Mark T-05 as complete in ROADMAP.md.
```

### Prompt 6 — Frontend Scaffold (T-06) — CAN RUN PARALLEL WITH T-05
```
Read tasks/T-06-frontend-scaffold.md. Set up the Vite React TS 
frontend: configure TailwindCSS with the green health/wellness color 
palette, set up TanStack Router with route definitions, configure 
TanStack Query provider, create the apiClient with JWT token 
management, set up i18next with Albanian (sq) as default locale, 
create the app shell layout with sidebar navigation. 
Verify `npm run build` and `npm run lint` pass.
Mark T-06 as complete in ROADMAP.md.
```

### Prompt 7 — Frontend Auth (T-07)
```
Read tasks/T-07-auth-frontend.md. Create the login page with 
react-hook-form + zod validation, implement the auth store in Zustand 
for token management, create the apiClient interceptor for automatic 
token refresh, add protected route wrapper that redirects to login if 
unauthenticated, create the user profile dropdown in the navigation.
Verify login flow works end-to-end with the backend.
Mark T-07 as complete in ROADMAP.md.
```

---

## Tips for Maximum Productivity

1. **One task per Claude session.** Start fresh with `/clear` between tasks to avoid context pollution.

2. **Use worktrees for parallel work.** Phase 0 has two parallel groups — use them.

3. **Let Claude read the task file.** Every prompt should start with "Read tasks/T-XX-name.md" so Claude has full context.

4. **Check status regularly.** Run `/project:status` to see what's unblocked.

5. **Expand stub tasks before implementing.** Many tasks in `tasks/` are stubs. Before starting a new phase, ask Claude to flesh out the task files:
   ```
   Read tasks/T-20-patients-api.md and expand it with detailed steps, 
   API endpoints, database schema, and test requirements — similar to 
   the level of detail in tasks/T-05-auth-backend.md. Don't implement 
   anything, just write the plan.
   ```

6. **Name your sessions.** Use `/rename` to name sessions like "T-14-dnd-calendar" so you can resume later with `/resume`.

7. **Review before merging.** After Claude finishes a worktree task, review the diff before merging to main.

---

## Expanding Task Files

Before starting each phase, flesh out the stub task files. Use this prompt:

```
I'm about to start Phase 2 (Patient Management & Walk-Ins). 
Read all task files from T-20 through T-28 in the tasks/ folder. 
For each stub task, expand it with:
- Detailed API endpoints (method, path, request/response)
- Database table definitions (if new tables needed)  
- UI component breakdown
- Specific test scenarios
- Step-by-step implementation checklist

Use the same level of detail as tasks/T-05-auth-backend.md 
and tasks/T-14-dnd-calendar.md. Don't implement anything — 
just write the detailed plans.
```

---

## Troubleshooting

**Claude doesn't know about the project:**
Make sure `CLAUDE.md` is in the repo root. Claude reads it automatically on session start.

**Subagent doesn't follow rules:**
The agent `.md` files in `.claude/agents/` must have proper YAML frontmatter with `---` delimiters.

**Worktree conflicts:**
Run `git worktree list` to see active worktrees. Clean up with `git worktree remove <path>`.

**Context window fills up:**
Use `/clear` to reset. For long tasks, split into smaller subtasks.

**Build failures after merge:**
Run `dotnet build` and `npm run build` after every merge. Fix conflicts before continuing.
