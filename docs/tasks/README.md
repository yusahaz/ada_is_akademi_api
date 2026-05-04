# Task and planning documents (`docs/tasks/`)

Markdown files in this folder are **working checklists** for features and phases. They supplement Cursor’s inline todo list and stay in git history.

## Conventions

| Convention | Detail |
|------------|--------|
| **Naming** | `phase1-identity-ilan.md` style: kebab-case, short and searchable. |
| **Structure** | Top: goal, in-scope / out-of-scope. Then ordered tasks (checkboxes `- [ ]` / `- [x]`). Optional: “Touch list”, “API”, “Risks”. |
| **Product name** | Use **Ada İş Akademi** in new docs. Older PRD files may still say “Worbi”; treat as legacy label unless updated. |
| **Deferred work** | Call out explicitly (e.g. “Faz 2 — raporlama”, “Sprint 2 — teknik risk”) so the agent does not expand scope silently. |

## Agent behavior

Follow the project rule **ada-is-akademi-agent-workflow** and skill **ada-is-akademi-plan**: analyze before large changes, update this folder when the user wants trackable delivery, preserve existing CQRS and layer patterns.
