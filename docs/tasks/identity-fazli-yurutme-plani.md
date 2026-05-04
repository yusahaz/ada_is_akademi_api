# Ada İş Akademi Identity Fazlı Yürütme Planı

## Hedef
- Identity akışlarını (`Register*`, `Login`, `RefreshToken`, `RequestEmailVerification`, `VerifyEmail`, `Me`) katman kurallarına uyumlu, güvenlik odaklı ve testlenebilir şekilde stabilize etmek.

## Kapsam
- Application: command/query/validator/handler davranış tutarlılığı.
- API: endpoint sözleşmesi, `AllowAnonymous` sınırı, `ApiResponse` zarfı.
- Test: kritik pozitif/negatif senaryoların doğrulanması.

## Kapsam Dışı
- MFA ve sosyal login entegrasyonları.
- Harici identity provider (OIDC/SAML) çalışmaları.
- Bildirim/e-posta altyapısı refaktörü.

## Faz 1 — Baseline ve Gap Analizi
- [x] `SystemUser` command/query seti için envanter çıkar.
- [x] `ServiceRegister` içinde handler/validator eşleşmelerini doğrula.
- [x] `SystemUsersController` için HTTP contract ve anonim endpoint doğrulaması yap.
- [x] Gap listesini 3 başlıkla sınıflandır: contract, validation, security.

## Faz 2 — Contract ve Validation Sertleştirme
- [x] Query response modellerinin `ModelBase` türetimini kontrol et ve eksikleri kapat.
- [x] `RequestSystemUserEmailVerification` için `ExpiresAt > UtcNow` zorunluluğunu validator katmanında uygula.
- [x] Identity validatorlarında zorunlu alan/pozitif id kontrollerini tekdüze hale getir.
- [x] Gerekirse `ApplicationValidationCodes` altında yeni kodları ekle.

## Faz 3 — Token Güvenliği ve Yaşam Döngüsü
- [x] Refresh akışında hesap durumu (`Active`) ve lock kontrolünü login ile hizala.
- [x] Device+refresh token eşleşmesi dışında token rotasyonu yapma.
- [x] Claim üretimi (`system_user_id`, `system_user_type`, `worker_id`, `employer_id`) tutarlılığını doğrula.
- [x] Hassas durumlarda sızıntı yapmayan hata yüzeyi (`NotFound`) korunmasını test et.

## Faz 4 — Test ve Kapanış
- [x] Validator seviyesinde identity negatif testlerini tamamla.
- [x] Handler seviyesinde refresh security negatif testlerini tamamla.
- [x] `dotnet test tests/ApplicationTests/ApplicationTests.csproj` ile regresyonu doğrula.
- [x] Kapanış notları (`Done / Follow-ups`) ile task dosyasını güncelle.

## Touch List (Beklenen)
- `src/Application/DependencyInjection/ServiceRegister.cs`
- `src/Application/ApplicationValidationCodes.cs`
- `src/Application/Commands/SystemUser/*.cs`
- `src/Application/Queries/SystemUser/*.cs`
- `src/Api/Controllers/SystemUsersController.cs`
- `tests/ApplicationTests/*.cs`

## Kabul Kriterleri
- Identity endpointleri sadece CQRS request body kabul eder ve `ApiResponse` zarfı döner.
- Refresh token yalnızca aktif + kilitsiz hesaplarda ve doğru device/token eşleşmesinde yenilenir.
- Identity validator testleri + kritik handler testleri geçer.
- Uygulama test projesi yeşil olur.

## Done / Follow-ups
- Done: `SystemUserMeModel` `ModelBase` ile hizalandı.
- Done: `RequestSystemUserEmailVerification` için `ExpiresAt` gelecek zaman doğrulaması ve ilgili validation code eklendi.
- Done: `RefreshSystemUserToken` akışında aktif + kilitsiz hesap zorunluluğu uygulandı.
- Done: Identity validator testleri ve refresh security negatif handler testi eklendi.
- Follow-up: Login ve refresh claim üretimi ortak bir instance servisinde birleştirilebilir.
