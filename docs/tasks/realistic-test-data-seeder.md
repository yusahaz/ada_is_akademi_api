# Gerçekçi test datası — SeedRunner

## Hedef

Staging/test PostgreSQL ortamında production hissiyatı için deterministik demo veri (worker, işveren, ilan, başvuru, payout, komisyon).

## Durum

- [x] `tools/AdaIsAkademi.SeedRunner` konsol projesi (Bogus `tr`, EF Core + Npgsql + proxies)
- [x] `InternalsVisibleTo` → `Domain`, `Persistence`, `Application` (`Azoxia.AdaIsAkademi.SeedRunner`)
- [x] CLI: `--reset`, `--workers`, `--employers`, `--open-postings`, `--closed-postings`, `--seed`, `--connection-string`, `--allow-production`
- [x] `ResetStage`: TRUNCATE + migration admin koruma + diğer kullanıcı DELETE
- [x] `LookupStage`: ~30 `[Seed]` iş kategorisi + `admin@adaisakademi.test`
- [x] `SkillCatalog`: 120 etiket (6 sektör × 20), `JobCategoryCatalog` ile hiyerarşi
- [x] `EmbeddingFaker`: SHA-256 tohumlu 1536 boyut L2 normalize vektör
- [x] `WorkforceStage`: worker + işveren + lokasyon + süpervizör
- [x] `JobPostingApplicationStage`: açık/kapalı ilanlar; kapalı tarihler SQL ile geçmişe alınır (domain başvuru kuralı)
- [x] `MonetizationStage`: WorkerPayout durum karışımı + CommissionReceivable (3 ay) + audit
- [x] `README.md` (araç klasöründe)
- [x] Bu görev dokümanı

## Notlar

- Embedding’ler OpenAI üretimi değildir; yalnızca semantic pipeline’ın boş kalmaması içindir.
- İdempotent çıkış: `worker001@adaisakademi.seed.local` mevcutsa `--reset` olmadan tekrar çalıştırma yapılmaz.
