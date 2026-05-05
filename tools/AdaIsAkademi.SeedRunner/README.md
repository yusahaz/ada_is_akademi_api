# Ada İş Akademi — gerçekçi demo veri (SeedRunner)

PostgreSQL üzerinde çalışan API veritabanına, test/staging ortamları için **deterministik** demo veri üretir (worker, işveren, ilan, başvuru, vardiya ataması, ödeme/komisyon akışı).

## Güvenlik

- **Production veritabanında çalıştırmayın.** `ASPNETCORE_ENVIRONMENT=Production` iken araç varsayılan olarak **çıkar**; zorunlu kalırsanız `--allow-production` ekleyin (önerilmez).
- Tüm seed hesapları `@adaisakademi.seed.local` ve `admin@adaisakademi.test` ile işaretlenir; ortak şifre: **`Ada!Test123`**.

## Ön koşullar

- Şema güncel olmalı (`dotnet ef database update` veya deploy pipeline).
- Bağlantı dizesi PostgreSQL içermeli (`Host`, `Database`, `Username`, `Password`).
- `appsettings.Seed.json` içinde örnek bağlantı vardır; production ile **paylaşmayın**.

## Çalıştırma

Repo kökünden:

```powershell
dotnet run --project tools/AdaIsAkademi.SeedRunner -- `
  --reset `
  --workers 100 `
  --employers 20 `
  --open-postings 50 `
  --closed-postings 50 `
  --seed 12345
```

Bağlantı önceliği:

1. `--connection-string "Host=...;..."`  
2. Ortam: `DOTNET_ConnectionStrings__AdaIs` veya `ConnectionStrings__AdaIs`  
3. `tools/AdaIsAkademi.SeedRunner/appsettings.Seed.json` → `ConnectionStrings:AdaIs`

## Parametreler

| Parametre | Varsayılan | Açıklama |
|-----------|------------|----------|
| `--reset` | kapalı | İşlem öncesi seed tablolarını temizler; **migration ile gelen** `admin@adaisakademi.local` kullanıcısı korunur. |
| `--workers` | 100 | Worker sayısı |
| `--employers` | 20 | İşveren sayısı |
| `--open-postings` | 50 | Açık (`Open`) ilan |
| `--closed-postings` | 50 | Kapalı (Completed / Filled / Cancelled karışımı) |
| `--seed` | 12345 | Bogus ve `System.Random` tohumu (tekrarlanabilirlik) |
| `--allow-production` | kapalı | Production ortam adında çalıştırmaya izin verir |

## Idempotent davranış

`worker001@adaisakademi.seed.local` kaydı **varsa** ve `--reset` **yoksa**, araç ikinci kez veri eklemeden çıkar. Yeniden üretmek için `--reset` kullanın.

## Üretilen içerik (özet)

- `[Seed]` önekli **hiyerarşik iş kategorileri** ve **120 adet** sektör skill etiketi.
- Worker profilleri: skill, ilgi kategorisi, müsaitlik, eğitim, deneyim, dil, isteğe bağlı sertifika/referans, **deterministik 1536 boyutlu sahte embedding** (OpenAI değildir).
- İşveren: komisyon oranı, 1–3 lokasyon, süpervizör kullanıcıları.
- İlanlar: skill gereksinimleri, ücret, başvurular (uyum ağırlıklı), kapalı akışlarda **QR → checkout** ve `WorkerPayout` + `CommissionAuditLog`; son **3 ay** için `CommissionReceivable`.

## Sorun giderme

- **Bağlantı hatası:** Docker kullanıyorsanız `Host=localhost` ve `Port` değerinin `.env` ile uyumlu olduğundan emin olun (ör. `15432`).
- **Şema uyumsuzluğu:** Önce API projesinde migration’ları uygulayın.
- **Temiz seed:** `--reset` migration admin dışındaki kullanıcıları da siler; yalnızca test DB için kullanın.
