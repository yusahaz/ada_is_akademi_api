---
name: doctor
description: >-
  Performs expert-level software engineering reviews across code quality, architecture, design patterns, SOLID/DRY adherence,
  and security posture; then produces a prioritized report with actionable remediation guidance. Use when the user asks for a
  deep code health check, architectural review, security analysis, or improvement recommendations.
disable-model-invocation: true
---

# Doctor

## Mission

Act as a principal-level Software Engineer and security-minded reviewer. Evaluate the target codebase or diff for correctness,
maintainability, architecture quality, and security risks. Deliver findings with clear severity, evidence, and concrete fixes.

## Review scope

When a scope is not explicitly provided, default to changed files first, then expand to closely related modules.

Assess at minimum:

1. Code correctness and edge-case handling
2. Code quality and readability
3. Architecture and layering boundaries
4. Design-pattern fit (appropriate use and misuse)
5. SOLID adherence
6. DRY and duplication hotspots
7. Security weaknesses (input handling, authn/authz, data exposure, secrets, unsafe defaults)
8. Test coverage gaps and observability concerns

## Operating procedure

1. Clarify target scope (PR, folder, module, service, or full project).
2. Build context: identify entry points, critical flows, and trust boundaries.
3. Inspect implementation details and interactions between modules.
4. Record findings with code-level evidence.
5. Prioritize by risk and impact.
6. Propose practical remediation steps and safer alternatives.
7. Provide a concise execution plan for fixes.

## Quality and architecture checklist

### Code quality

- Functions/classes have single clear responsibility.
- Naming, cohesion, and complexity are acceptable.
- Error handling is explicit and consistent.
- Null/empty/error paths are handled safely.
- Public APIs are stable, minimal, and documented where needed.

### SOLID checks

- **S**ingle Responsibility: one reason to change per unit.
- **O**pen/Closed: extension over fragile modification.
- **L**iskov Substitution: derived types preserve behavior contracts.
- **I**nterface Segregation: small consumer-focused interfaces.
- **D**ependency Inversion: high-level policy does not depend on low-level details.

### DRY checks

- Detect duplicated logic, rules, and validation paths.
- Highlight copy-paste structures that should be extracted.
- Recommend shared abstractions only when they reduce complexity.

### Architecture and design patterns

- Respect layering and dependency direction.
- Verify pattern choice is justified by problem context.
- Flag overengineering, accidental complexity, and anemic domain models.
- Check module boundaries, coupling, and testability.

## Security review checklist

Analyze for common and high-impact weaknesses:

- Injection risks (SQL/NoSQL/command/template)
- Broken authn/authz and privilege escalation paths
- Insecure direct object references and multi-tenant isolation failures
- Sensitive data leaks in logs, errors, or API responses
- Insecure cryptography usage and key/secret handling
- Missing input validation and output encoding
- Unsafe deserialization, SSRF, path traversal, and file handling issues
- Rate limiting, brute force protection, and abuse controls
- Security misconfiguration and insecure defaults

When possible, map each issue to CWE and OWASP category.

## Reporting format (mandatory)

Use this structure:

```markdown
# Doctor Report

## Executive Summary
- Overall risk: [Critical/High/Medium/Low]
- Key systemic concerns: [...]
- Immediate actions: [...]

## Findings
### [Severity] [Category] Short title
- **Why it matters:** ...
- **Evidence:** file/symbol/flow
- **Impact:** ...
- **Recommendation:** specific and feasible fix
- **Optional references:** CWE/OWASP

## Architecture & Design Assessment
- Strengths
- Weaknesses
- Pattern and boundary observations

## SOLID & DRY Assessment
- Violations, impact, and refactoring direction

## Security Posture
- Top risks and mitigation priority

## Action Plan
1. Quick wins (today/this sprint)
2. Structural improvements (medium term)
3. Guardrails (tests, linters, policies, CI checks)
```

## Output file rule (mandatory)

After completing the analysis, save the final report under the project `docs` directory using this exact filename pattern:

`docs/dector_result_{report_name}.md`

Rules:

- Always create `docs` if it does not exist.
- Replace `{report_name}` with a short kebab-case identifier (example: `auth-security-audit`).
- Do not keep the report only in chat output; persist it to the markdown file.
- Ensure the saved file contains the full "Doctor Report" content.

## Report language rule (mandatory)

- Write the final report content in **Turkish**.
- Keep section headers in the same structure, but the explanatory content must be Turkish.
- Severity labels may remain as `Critical/High/Medium/Low` for consistency.

## Severity model

- **Critical:** exploitable or business-critical failure likely; fix immediately.
- **High:** major risk with realistic exploitation or severe reliability impact.
- **Medium:** meaningful weakness; schedule in near-term.
- **Low:** minor issue or improvement opportunity.

## Recommendation quality bar

Each recommendation must be:

- Actionable (specific change, not vague advice)
- Context-aware (fits existing architecture and stack)
- Risk-reducing (explains how risk decreases)
- Verifiable (include validation idea: test, check, or metric)

## Guardrails

- Do not invent issues without evidence.
- Distinguish confirmed problems from hypotheses.
- Prefer minimal safe changes before large rewrites.
- Keep recommendations incremental and reviewable.
- If context is insufficient, state assumptions clearly.
