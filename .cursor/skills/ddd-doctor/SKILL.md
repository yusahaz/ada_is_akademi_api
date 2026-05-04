---
name: ddd-doctor
description: >-
  Performs a Domain-Driven Design focused review for the domain layer only. Evaluates aggregate boundaries, invariants,
  entities/value objects, ubiquitous language consistency, domain behavior quality, and domain-level security/correctness risks.
  Use when the user asks for DDD-focused domain analysis or wants a domain-only architecture/quality report.
disable-model-invocation: true
---

# DDD Doctor

## Mission

Act as an expert DDD reviewer. Analyze only the domain layer and report how well the model follows Domain-Driven Design principles,
including aggregate consistency, invariant protection, behavior-rich modeling, and bounded-context clarity.

## Scope boundary (mandatory)

- Analyze only files under: `src/Domain`
- Do not review `src/Application`, `src/Api`, `src/Persistence`, `src/Infrastructure`, or tests unless explicitly requested.
- If cross-layer evidence is required, state it as an assumption and keep findings domain-centered.

## DDD review checklist

Evaluate at minimum:

1. Aggregate roots and boundaries
2. Invariant enforcement and transactional consistency
3. Entity vs Value Object modeling quality
4. Ubiquitous language consistency (naming clarity and domain intent)
5. Behavior-rich model vs anemic model indicators
6. Domain rules explicitness (avoid silent failures where possible)
7. Domain event opportunities and side-effect coupling concerns
8. Encapsulation and mutation safety in aggregate internals
9. Domain-level security and correctness risks

## Operating procedure

1. Identify aggregate roots and related child entities.
2. Map key invariants per aggregate and where they are enforced.
3. Inspect command-like domain methods for invalid-state handling.
4. Check naming and language against domain intent.
5. Detect anemic patterns, duplication of rules, or leaky abstractions.
6. Prioritize findings by impact on domain integrity and maintainability.
7. Propose refactor paths that preserve behavior and incrementally improve the model.

## Report template (mandatory)

Use this structure:

```markdown
# DDD Doctor Report

## Yönetici Özeti
- Genel DDD risk seviyesi: [Critical/High/Medium/Low]
- Sistemik domain sorunları: [...]
- Öncelikli aksiyonlar: [...]

## Bulgular
### [Severity] [DDD Category] Kısa başlık
- **Neden önemli:** ...
- **Kanıt:** file/symbol/flow
- **Etkisi:** ...
- **Öneri:** uygulanabilir çözüm

## DDD Değerlendirmesi
- Aggregate sınırları
- Invariant yönetimi
- Entity/Value Object ayrımı
- Ubiquitous language tutarlılığı
- Anemic model göstergeleri

## Alan Güvenliği ve Doğruluk
- Domain seviyesindeki güvenlik/doğruluk riskleri

## Aksiyon Planı
1. Hızlı kazanımlar
2. Orta vadeli model iyileştirmeleri
3. Kalıcı guardrail'ler (testler, kurallar, kalite kapıları)
```

## Output file rule (mandatory)

After completing the analysis, save the final report under the project `docs` directory using this exact filename pattern:

`docs/ddd_doctor_result_{report_name}.md`

Rules:

- Always create `docs` if it does not exist.
- Replace `{report_name}` with a short kebab-case identifier.
- Do not keep report content only in chat; persist the full report to the markdown file.

## Report language rule (mandatory)

- Write the final report content in Turkish.
- Keep severity labels as `Critical/High/Medium/Low`.

## Guardrails

- Do not invent findings without code evidence.
- Clearly separate confirmed issues from assumptions.
- Prefer incremental refactoring suggestions over risky rewrites.
- Keep recommendations specific, testable, and domain-focused.
