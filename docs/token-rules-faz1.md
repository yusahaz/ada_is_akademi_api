# Ada İş Akademi — token kuralları (Faz 1 gerçeği vs PRD v6)

Kaynak PRD: `docs/worbi_prd_v6.md` (§4 e-posta / §4.4 JWT). Bu belge **repoda şu an uygulanan** davranış ile PRD hedefini karşılaştırır.

## E-posta doğrulama token’ı (`SystemUser`)

| Konu | PRD v6 | Bu repo (Faz 1) |
|------|--------|------------------|
| Saklama | SHA-256 hash DB’de; ham token e-postada | `RequestEmailVerification` hash + süre; `VerifyEmail` eşleştirme + süre |
| Tek kullanım | Tek kullanımlık | Başarılı doğrulamada alanlar sıfırlanır; aynı hash ile ikinci kez başarı beklenmez |
| Süre | Diyagramda “24s” ifadesi (muhtemelen 24 saat); PRD metni “süresi dolmuşsa yeni token” | **Uygulama:** `ExpiresAt` ile mutlak süre; dolunca yeni `RequestEmailVerification` |
| PRD farkı | — | Üretim süresi değerini ürününüz netleştirin; diyagramdaki süre metni PRD ile kod aynı dilde teyit edilmeli |

## JWT access / refresh (PRD §4.4)

| Konu | PRD v6 hedefi | Bu repo (Faz 1) |
|------|----------------|------------------|
| Access token | 15 dk, RS256 | **Yerel/HMAC:** `Azoxia.Core.Api` `Startup` + `JwtConfig` (`Key`/`Issuer`/`Audience`/`ExpireMinutes`); süre **üretilen token’ın `exp` claim’ine** bağlı (login uçları ayrı teslimat; test için harici JWT) |
| Refresh token | 30 gün, hash, `UserSession`, rotation | **Entity mevcut:** `SystemUserRefreshToken`; **akış (üret/yenile/revoke)** Sprint 2 / ayrı iş parçası |
| Claim’ler | (PRD şema ayrıntısı) | API: `employer_id`, `worker_id` — Application’da `IExecutionContext.GetClaim(...)` ile okunur; **işveren / işçi aktör kimliği yalnızca claim ile** bağlanır (ilgili DTO’larda `employerId` / `workerId` alanı yoktur) |

## Özet

- **E-posta token:** Domain + komutlar PRD ile uyumlu; süre sabiti ürün kararına göre tek yerden yönetilmeli.
- **JWT:** Faz 1’de API tarafı **doğrulama + claim bağlama** tamam; **token üretimi**, RS256 ve refresh rotation **sonraki teslimat**.
- **Ertelenen:** Çoklu cihaz oturum ayrıntısı, FCM, Redis tabanlı izin önbelleği — `docs/tasks/phase1-identity-jobposting-application.md` §2/§7 ile hizalı.
