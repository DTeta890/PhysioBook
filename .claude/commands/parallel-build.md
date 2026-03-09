# Parallel Build

## Arguments
$ARGUMENTS — Comma-separated task IDs (e.g., "T-10, T-11, T-13") OR a phase name (e.g., "phase-1-group-a")

## Instructions

1. Parse $ARGUMENTS to identify which tasks to run in parallel
2. For each task, verify:
   - All dependencies are ✅
   - No file overlap between the tasks (check domains)
3. If tasks overlap in files, STOP and suggest sequential ordering instead
4. For each task, create a subagent with `isolation: worktree`:
   - Assign the correct domain agent (backend-developer, frontend-developer, database-engineer)
   - Each agent works in its own worktree: `.claude/worktrees/{TASK-ID}/`
   - Provide each agent with: the task file contents, relevant CLAUDE.md rules, verification requirements
5. Launch all agents in parallel using the Task tool with `run_in_background: true`
6. Monitor progress — check `.agent-status/` for completion
7. When all agents complete:
   - Review each agent's changes
   - Run full verification suite
   - Merge branches sequentially (resolve any conflicts)
   - Update all task statuses in `ROADMAP.md` to ✅
8. Report results summary
