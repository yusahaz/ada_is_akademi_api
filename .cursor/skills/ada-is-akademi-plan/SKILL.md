---
name: ada-is-akademi-plan
description: >-
  Runs a short analysis before implementation for Ada İş Akademi, optionally writes or updates a
  markdown task checklist under docs/tasks/, and aligns execution with layer rules and existing CQRS
  patterns. Use when the user asks for a plan, task breakdown, sprint prep, or non-trivial feature work
  on this repo.
---

# Ada İş Akademi — plan and task tracking

## When to use

- User asks for a plan, task list, sprint breakdown, or “what to do first”.
- Change touches **Domain + Application + Persistence + Api** or adds a new use-case.
- User wants work documented in-repo for later review.

## Workflow

1. **Clarify scope** in one pass: product area (Identity / Job posting / Application / …), in-scope vs explicitly deferred (e.g. reporting = later phase, infra risks = sprint 2).
2. **Read** relevant `.cursor/rules/*.mdc` for layers you will modify.
3. **Inspect** existing similar commands, queries, entities, and `ServiceRegister` registrations — **copy the same structure** for new features.
4. **Write or update** `docs/tasks/<short-kebab-name>.md`:
   - Goal and out-of-scope bullets
   - Numbered or checkbox tasks ordered by dependency
   - Optional: files/APIs to touch, validation and persistence notes
   - Link to PRD sections only when useful (`docs/worbi_prd_v6.md` is historical naming; prefer “Ada İş Akademi” in new text)
5. **Execute** using the editor todo list for live steps; keep the markdown file in sync when tasks complete or scope shifts.
6. **Do not** expand scope into deferred areas without user confirmation.

## Task file location

- Directory: `docs/tasks/`
- Naming: `docs/tasks/<feature-or-phase>-<short-description>.md` (kebab-case).
- See `docs/tasks/README.md` for conventions.

## After implementation

- If behavior or public contract changed, ensure `ServiceRegister` and any new validators/handlers are registered.
- Prefer a short “Done / follow-ups” section at the bottom of the task file when closing a chunk of work.
