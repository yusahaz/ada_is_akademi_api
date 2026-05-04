---
name: application-cqrs-doctor
description: >-
  Reviews the Application layer with awareness of the Core CQRS infrastructure (commands, queries, handlers, pipelines, unit of work)
  and validates collaboration boundaries with the Domain layer. Use when auditing application services, handlers, use-cases,
  or when the user asks for CQRS-focused analysis and improvement recommendations.
disable-model-invocation: true
---

# Application CQRS Doctor

## Mission

Act as a senior software engineer specialized in CQRS application architecture. Analyze the Application layer using the project's
existing Core CQRS conventions, and evaluate how use-cases coordinate with Domain entities and rules.

## Scope boundary (mandatory)

- Primary scope: `src/Application`
- Supporting context allowed: `src/Domain` and Core CQRS abstractions used by handlers
- Avoid broad infrastructure/API review unless explicitly requested

## CQRS-aware checklist

Evaluate at minimum:

1. Command/Query separation correctness
2. Handler responsibilities and SRP
3. Transaction boundaries (`UnitOfWork` usage, save timing, side effects)
4. Validation completeness before domain mutation
5. Domain collaboration quality (no business logic leakage into handlers)
6. Error handling strategy and consistency of error codes/exceptions
7. Idempotency and duplicate-request safety where relevant
8. Testability of handlers and use-case orchestration

## Core pipeline awareness

When reviewing, assume the project follows Core abstractions such as:

- `CommandBase`, `QueryBase` (or equivalent request models)
- `CommandHandlerBase<T>`, `QueryHandlerBase<TQuery, TResult>`
- Mediator-style dispatching
- Unit of Work + Repository coordination

Use these assumptions to reason about what belongs in:

- Request model
- Validator / precondition layer
- Handler orchestration
- Domain entity behavior

## Domain collaboration rules

- Handlers should orchestrate, not own core business rules.
- Domain invariants must stay in Domain entities/value objects.
- Application layer may enforce use-case policies (authorization/context), but not duplicate deep domain invariants.
- If domain rule enforcement is missing, recommend moving logic to Domain rather than growing handler complexity.

## Review workflow

1. Identify command/query entry points and handler pairs.
2. Trace each use-case from request input to domain mutation and persistence.
3. Verify validation, guardrails, and transactional consistency.
4. Flag leakage: business rules in handlers, persistence details in request models, or domain bypasses.
5. Produce prioritized findings with concrete code-level fixes.

## Report template (mandatory)

Use this structure:

```markdown
# Application CQRS Doctor Report

## Yönetici Özeti
- Genel risk seviyesi: [Critical/High/Medium/Low]
- Sistemik CQRS/Application sorunları: [...]
- Öncelikli aksiyonlar: [...]

## Bulgular
### [Severity] [Category] Kısa başlık
- **Neden önemli:** ...
- **Kanıt:** file/symbol/flow
- **Etkisi:** ...
- **Öneri:** uygulanabilir çözüm

## CQRS Uygunluk Değerlendirmesi
- Command/Query ayrımı
- Handler sorumluluk dağılımı
- Transaction / UnitOfWork yönetimi
- Validation ve hata stratejisi

## Domain İşbirliği Değerlendirmesi
- Domain kuralı sızıntıları
- Uygulama katmanında olması gereken vs olmaması gereken kurallar

## Aksiyon Planı
1. Hızlı kazanımlar
2. Orta vadeli iyileştirmeler
3. Kalıcı guardrail'ler (testler, checklist, CI kalite kapıları)
```

## Output file rule (mandatory)

After analysis, save the report into:

`docs/application_cqrs_doctor_result_{report_name}.md`

Rules:

- Create `docs` if missing.
- Use short kebab-case for `{report_name}`.
- Persist the full report to file, not chat-only.

## Report language rule (mandatory)

- Final report content must be in Turkish.
- Severity labels can remain `Critical/High/Medium/Low`.

## Guardrails

- Do not invent CQRS infrastructure behavior without evidence.
- Clearly distinguish confirmed issues from assumptions.
- Prefer incremental refactoring over large rewrites.
- Keep recommendations compatible with existing Core abstractions.
