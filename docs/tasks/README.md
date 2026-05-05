# Görev ve plan dokümantasyonu (`docs/tasks/`)

Bu klasör, Ada İş Akademi için **sprint/epik takibi**, çok adımlı işlerin checklist’i ve `/ada-is-akademi-plan` ile uyumlu plan kalıntılarını tutar.

## Tek kaynak sırası

1. **`ada-is-akademi-execution-tracker.md`** — güncel faz, “şimdi ne”, work log (sık güncellenir).
2. **`docs/tasks/<konu>.md`** — belirli bir özellik veya gözden geçirme için detaylı checklist (ör. `worker-employer-profile-enrichment.md`).
3. **`docs/worbi_prd_v6.md`** — tarihsel PRD adı; yeni metinlerde ürün adı olarak **Ada İş Akademi** tercih edilir.

## Dosya adlandırma

- **Biçim:** `kebab-case`, `<özellik-kısım>-<kısa-açıklama>.md`.
- **Örnek:** `codebase-structure-review-2026-05.md`, `worker-employer-profile-enrichment.md`.

## Ne zaman yeni dosya açılır?

- Domain + Application + Persistence + Api’yi kesen yeni kullanım öyküsü.
- Çok adımlı refaktör veya mimari envanter (bkz. `.cursor/skills/ada-is-akademi-plan/SKILL.md`).
- Ürün ekibiyle paylaşılacak “definition of done” checklist’i gerektiğinde.

## Kapanışta

- İlgili checklist kutularını işaretleyin.
- `ada-is-akademi-execution-tracker.md` içinde **Current Status** ve gerekiyorsa **Work Log** güncellenir.
- Davranış veya HTTP sözleşmesi değiştiyse `ServiceRegister` ve API/OpenAPI notları gözden geçirilir.
