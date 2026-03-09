# Implement Task

## Arguments
$ARGUMENTS — The task ID (e.g., T-12) or task description to implement.

## Instructions

1. Find the task in `ROADMAP.md` matching $ARGUMENTS
2. Check dependencies — if any dependency is ⬜, STOP and report which tasks must be completed first
3. Read the detailed task file from `tasks/` if it exists
4. Create a git branch: `feature/{TASK-ID}-{short-description}`
5. Determine required agents:
   - If task involves API/backend → use `backend-developer` agent
   - If task involves UI/components → use `frontend-developer` agent
   - If task involves schema/migration → use `database-engineer` agent
   - If task spans multiple domains → dispatch parallel agents per domain
6. Implement the task following the agent's rules
7. Run ALL verification checks from CLAUDE.md:
   - `dotnet build` (if backend changed)
   - `dotnet test` (if backend changed)
   - `npm run build` (if frontend changed)
   - `npm run lint` (if frontend changed)
8. If any check fails, fix the issue before proceeding
9. Update `ROADMAP.md`: change task status from ⬜ to ✅
10. Commit: `feat: {task description} ({TASK-ID})`
11. Report completion summary
