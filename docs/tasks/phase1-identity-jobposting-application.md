# Faz 1 — Kimlik + İlan + Başvuru (Ada İş Akademi)

**Durum:** Görev listesi güncellendi (`ada-is-akademi-plan` — 2026-05-03)  
**Kapsam (Faz 1):** Identity + ilan + başvuru.  
**Kapsam dışı (bilinçli):** Raporlama → **Faz 2**. Teknik risk yoğun işler (QR/Redis, pessimistic payout, pgvector, agentic bildirim vb.) → **Sprint 2+**.  
**Domain:** Mevcut `src/Domain` ve CQRS yapısı ile uyumlu ilerle; PRD (`docs/worbi_prd_v6.md`) tarihsel isimlendirme içerebilir; yeni metinlerde **Ada İş Akademi**.

## Repo gerçeği (plan skill — kısa snapshot)

- **Application:** `CreateJobPosting`, `Submit` / `Withdraw`, ilan yaşam döngüsü ve başvuru komutları `ServiceRegister`’da. `IServiceRegister.Register(IServiceCollection, IConfiguration)` uyumlu. Okuma sonuçları **`ModelBase` türevi `*Model`** kayıtları (komut/query klasöründe ayrı `.cs` dosyaları; `.cursor/rules/application-layer.mdc`). Doğrulama kodları `ApplicationValidationCodes` (`ErrorCode`). **Önbellek:** `ICacheService` ile detay sorgularında cache-aside, ilgili komutlarda `InvalidateByDependencyAsync` — anahtarlar `Caching/AdaIsCacheKeys.cs`.
- **Başvuru:** Submit / Withdraw / Accept / Reject + `ListJobApplicationsByJobPostingIdQuery` mevcut; ilan sahibi işlemlerinde aktör **`employer_id`** claim’inden; başvuru / çekmede **`worker_id`** claim’inden türetilir; handler’da `JobPosting.EmployerId` ile eşleşme (uyumsuzluk → `NotFound`). Eksik/geçersiz claim’ler `AZX_ADA_APP_VAL_900` / `901`. **DTO’larda `EmployerId` / `WorkerId` yoktur.**
- **API:** `ApiControllerBase` **`[Authorize]`**; **kimliksiz katalog:** `JobPostings/GetById`, `JobPostings/ListOpen`, **`SystemUsers`** e-posta uçları `[AllowAnonymous]`. **JWT:** Core `Startup` + `JwtConfig` + `AddAzoxiaJwtBearerAuthentication`; claim erişimi Application katmanında `IExecutionContext`. **`ListJobPostingsByEmployerIdQuery`** → `JobPostings/ListByEmployer`. HTTP gövdeleri **Application komut/sorgu** (`[FromBody]`). OpenAPI: `Tags` / `Produces` / `EndpointSummary` — `.cursor/rules/api-layer.mdc`.
- **Persistence:** `Mapping/JobApplicationConfiguration` — Pending için `(JobPostingId, WorkerId)` filtreli unique index (migration üretildiğinde sütun adı doğrulanmalı).
- **Testler:** `tests/DomainTests` geçiyor. `tests/ApplicationTests`: **`AdaIsCacheKeysTests`**, **`JobPostingApplicationValidatorsTests`** (işveren doğrulamaları + `ListJobPostingsByEmployerId`), **`Phase1CriticalCommandHandlersTests`** (SQLite).

### Önerilen uygulama sırası (bağımlılık) — güncel

1. ~~Domain + `CreateJobPostingCommand` + HTTP~~ (tamamlandı.)
2. ~~Submit / Withdraw + persistence unique + API~~ (tamamlandı.)
3. ~~JWT + granular `[AllowAnonymous]`; `employer_id` / `worker_id` → yalnızca `IExecutionContext` (Komut/query gövdesinde aktör id yok); işveren ilan listesi (`ListByEmployer`); token kuralları (`docs/token-rules-faz1.md`).~~ **2026-05-04:** tamamlandı.
4. **Sonraki (Faz 2 / Sprint 2):** login/token issuance uçları, RS256/IdP, refresh rotation + `UserSession`, RBAC policy, PostgreSQL skill unique index migration, Swagger UI (ürün kararına göre).

---

## 0. Çıkış ölçütleri (MVP tanımı)

- [x] Worker benzeri kullanıcı: **Faz 1 kapsamı** — `SystemUser` satırı (ör. seed / iç araç / ileride açılacak kayıt komutu) + **`RequestSystemUserEmailVerification` / `VerifySystemUserEmail`** API ve domain; uçtan uca doğrulama handler testi (`VerifySystemUserEmailHandler_activates_user_when_token_matches`). **Ayrı self-service “Register” komutu** ürün kararına göre sonraki iş.
- [x] İşveren tarafı: ilan oluşturma → taslak → yayın (`JobPosting` yaşam döngüsü) API üzerinden kullanılabilir (`JobPostingsController`; persistence migration üretildiyse unique index doğrulanmalı).
- [x] Worker: açık ilana **başvuru** oluşturma (`SubmitJobPostingApplicationCommand` + withdraw; domain ile uyumlu).
- [x] İşveren: başvuruları listeleme / kabul / red — JWT **`employer_id`** ile handler `JobPosting.EmployerId` eşlemesi; handler/validator testleri.
- [x] Kalite: `AdaIsCacheKeysTests`, `JobPostingApplicationValidatorsTests`, **Faz 1 kritik handler testleri** (`CreateJobPosting`, `Submit` idempotent, `Accept`, `List`, `VerifyEmail`), domain’de **çift başvuru** (`AddApplication` aynı worker).

---

## 1. Analiz ve hizalama (işe başlamadan önce)

- [x] `docs/worbi_prd_v6.md` içinden **Faz 1 MVP** maddeleri ile bu dosyadaki kapsamı karşılaştır; fark varsa bu bölüme not düş.
- [x] Mevcut entity haritası: `SystemUser`, `Worker`, `Employer`, `JobPosting`, `JobApplication`, `JobCategory` — başvuru ve ilan için ERD benzeri kısa not (isteğe bağlı `docs/tasks/` altında veya yorum).
- [x] API yüzeyi: hangi controller/route seti (mevcut `AssignmentController` genişletilecek mi, yeni `JobPostingsController` mı) — karar ve gerekçe bir cümle.

### 1.1 PRD Faz 1 MVP vs Ada İş Akademi Faz 1 (bu dosya)

| PRD §14 Faz 1 maddesi | Bu repodaki Faz 1 kararı |
|------------------------|---------------------------|
| Identity: kayıt, email doğrulama, JWT, multi-device session, UserGroup RBAC, PermissionResolver | **Dahil (minimum):** kayıt + email doğrulama mevcut domain/komutlarla hizalama ve API. **Sprint 2+ / ayrı iş:** multi-device `UserSession` ayrıntısı, tam RBAC seed/migration, PermissionResolver derinliği. |
| Migration: seed permissions, sistem grupları, admin | **Kısmi:** JobCategory seed Faz 1’de yeterli olabilir; tam permission seed **ürün kararına** bağlı (MVP’de basit auth yeterliyse ertelenebilir). |
| Worker profili: tüm bölümler + CV pipeline | **Daraltılmış:** başvuru için `Worker` + `SystemUser` yeteri; profil/CV tamamı **Faz 2 / sonraki**. |
| Employer profili, lokasyon, supervisor | **Dahil (mevcut domain):** ilan için `Employer`, `EmployerLocation` zorunlu; supervisor yönetimi MVP’de sadece ihtiyaç varsa. |
| JobCategory + seed | **Dahil** (salt okunur seed yeterli — Bölüm 3 kararı). |
| İlan oluşturma, başvuru akışı | **Dahil** (çekirdek; `CreateJobPosting` + submit/withdraw + API — **yetki/JWT** Faz 1’de açık). |
| Komisyon motoru, Mutual QR, CommissionReceivable + PDF, WorkerPayout | **Kapsam dışı** (kullanıcı: teknik risk Sprint 2+; raporlama Faz 2). |
| Flutter uygulamaları, Admin panel | **API öncelikli**; client/admin ayrı teslimat. |

### 1.2 İlan / başvuru — kısa ilişki (mevcut domain)

```
SystemUser ──1:1── Worker
Employer ──1:N── EmployerLocation
Employer ──1:N── JobPosting ──N:1── JobCategory
JobPosting ──1:N── JobApplication ──N:1── Worker
```

- Başvuru satırı: `JobApplication(JobPostingId, WorkerId, …)`; worker tarafı `Worker.SystemUserId` üzerinden kimliğe bağlanır.
- İlan oluşturma: **`Employer.AddJobPosting(...)`** — yalnızca `EmployerStatus.Active` iken; lokasyon bu işverenin `Locations` listesinde olmalı; taslak `JobPosting` koleksiyona eklenir (`src/Domain/Entities/Employer.cs`).

### 1.3 API controller stratejisi (karar)

- **Yeni, alan bazlı controller’lar:** `JobPostingsController`, `JobApplicationsController`, kimlik ve kullanıcı uçları için `SystemUsersController` veya `AuthController` (Core pipeline ile uyumlu isimlendirme tercih edilir).
- **`AssignmentController` genişletilmesin:** adı “atama”yı çağırıyor; ilan/başvuru REST sınırlarıyla örtüşmüyor; şu an boş — ya kaldırılır ya da tek bir legacy yönlendirme için bırakılır; yeni endpoint’ler buraya eklenmez.
- **Gerekçe:** Route öngörülebilirliği, CQRS komut başına dosya sınırı, ileride Assignment (vardiya atama) ayrı bounded context olduğunda isim çakışması olmaz.

### 1.4 Domain kararı — `JobPosting` üretimi (kilitleme)

- **Tek giriş noktası:** `Employer.AddJobPosting` (aggregate kökü). Dışarıdan `new JobPosting(...)` kullanılmaz; Application katmanı işvereni yükleyip bu metodu çağırır.
- **Kurallar (özet):** işveren `Active`; `employerLocationId` bu işverene ait bir lokasyon `Id` olmalı; `shiftEndTime > shiftStartTime`; `headCount > 0`. `JobCategory` varlığı komut/validator katmanında doğrulanacak.

---

## 2. Kimlik (Identity)

- [x] Email doğrulama: `RequestSystemUserEmailVerificationCommand` / `VerifySystemUserEmailCommand` + `SystemUser.RequestEmailVerification` / `VerifyEmail` — `Pending` → doğrulama sonrası `Active`; başarılı doğrulamada **e-posta doğrulama token/süre alanları sıfırlanır**. HTTP: `ApiControllerBase` yolu (`SystemUsers/RequestEmailVerification`, `SystemUsers/VerifyEmail`) + JSON gövdesinde `SystemUserId` / `TokenHash`.
- [x] Token kuralları (süre, tek kullanım, hash): `docs/token-rules-faz1.md` — `SystemUser` e-posta token’ı + PRD §4.4 JWT/refresh hedefi vs repo gerçeği; refresh akışı sonraki sprint.
- [ ] **Sprint 2’ye ertelenen:** çoklu cihaz `UserSession`, FCM, JWT rotation detayları — bu dosyada sadece madde başlığı, implementasyon yok.

---

## 3. İlan (Job posting)

- [x] **`CreateJobPosting` (Application):** `CreateJobPostingCommand` + validator + handler + `ServiceRegister` — `src/Application/Commands/JobPosting/CreateJobPostingCommand.cs`.
- [x] **`CreateJobPosting` (HTTP):** `JobPostings/Create` + `CreateJobPostingCommand` gövdesi (ayrı Api DTO yok).
- [x] Güncelleme / yayınlama / iptal / tamamlama / liste / detay: `JobPostingsController` (`[AllowAnonymous]` geçici).
- [x] `JobCategory` yönetimi: **Faz 1 kararı —** MVP’de salt okunur **seed + mevcut `JobCategory` doğrulaması** (`CreateJobPosting` vb.) yeterli; **ayrı admin yazma API’si Faz 1 kapsamı dışı** (gerekirse sonraki faz / iç araç).
- [x] İlan listeleme: açık ilan vitrin için `ListOpenJobPostings` + detay `GetJobPostingById` yeterli; **işveren paneli** için **`ListJobPostingsByEmployerIdQuery`** + HTTP `JobPostings/ListByEmployer` eklendi (`ServiceRegister` dahil).

---

## 4. Başvuru (Application)

- [x] **Submit / withdraw:** `SubmitJobPostingApplicationCommand` (`CommandBase<int>`) + `WithdrawJobPostingApplicationCommand` + `ServiceRegister`.
- [x] DB: `JobApplicationConfiguration` — Pending için unique index (migration henüz üretilmediyse `dotnet ef migrations add` ile doğrula).
- [x] `Accept` / `Reject` komutları: orkestrasyon incelemesi — handler yükleme → **işveren eşlemesi** → `JobPosting.AcceptApplication` / `RejectApplication` (kapasite, durum, başvuru koleksiyonunda `applicationId` eşlemesi domain’de); yanlış `JobPostingId`+`ApplicationId` çifti domain’de `JobApplicationNotFound`. Ek iş kuralı gerekmez.
- [x] `ListJobApplicationsByJobPostingIdQuery` / Accept / Reject: **işveren doğrulaması** — handler’da JWT **`employer_id`** ↔ `posting.EmployerId` (aksi `NotFound`); claim yok/geçersiz → `AZX_ADA_APP_VAL_900`.

---

## 5. API katmanı

- **Front-end rehberi (Faz 1):** `docs/api-frontend-faz1.md`
- [x] Alan bazlı controller’lar: `JobPostings`, `JobApplications`, `SystemUsers` (eski boş `AssignmentController` kaldırıldı).
- [x] Route: **`[controller]/[action]`** (`ApiControllerBase`); mutlak `/api/...` şablonları yok. İstek gövdeleri Application **komut/sorgu** tipleri (`[FromBody]`); ayrı Api `Models` klasörü yok.
- [x] `Program` / CORS / auth: **JWT Bearer** (`JwtConfig` + Core `AddAzoxiaJwtBearerAuthentication`) + claim çözümü için **`IExecutionContext`**; kimliksiz uçlar için **`[AllowAnonymous]`** yalnızca `GetById` / `ListOpen` / `SystemUsers`. Core `Startup` içinde **`UseAuthentication`** pipeline sırası düzeltildi. CORS üretim politikası ayrı iş.

---

## 6. Kalite ve izlenebilirlik

- [x] `tests/ApplicationTests`: `AdaIsCacheKeysTests` — cache anahtarı/dependency adlandırma (3 test).
- [x] `tests/ApplicationTests`: `JobPostingApplicationValidatorsTests` — list/accept/reject validator yolları (claim tabanlı aktör; DTO’da işveren id yok).
- [x] `tests/ApplicationTests`: kritik handler senaryoları — `Phase1CriticalCommandHandlersTests` (SQLite in-memory, `IExecutionContext` + `ICacheService` test sınıfları).
- [x] `tests/DomainTests`: çift başvuru (`AddApplication` aynı worker); email + ilan durumu mevcut testlerle Faz 1 ile uyumlu.
- [x] `docs/application_cqrs_doctor_result_post-queries.md` (ve baseline üst notu) güncel repo ile senkron — 2026-05-03; ileride DI değişirse tekrar hizala.

### Done / follow-ups (2026-05-03 — Application katmanı chunk)

- **Tamamlandı (özet):** Okuma sonuçları `ModelBase` + ayrı `*Model` dosyaları; `ApplicationValidationCodes` (`ErrorCode`); `AdaIsCacheKeys` + ilgili Get*ById cache-aside ve komut sonrası invalidation; `.cursor/rules/application-layer.mdc` önbellek kuralları; `AdaIsCacheKeysTests` + `InternalsVisibleTo` (gerekliyse). **İşveren–ilan eşlemesi:** liste / kabul / red için `employer_id` claim (`ExecutionContextAdaIsExtensions`) ve `JobPostingApplicationValidatorsTests`. **2026-05-05:** Aktör kimlikleri yalnızca claim (`AZX_ADA_APP_VAL_900` / `901`); Withdraw’da işçi eşlemesi. **2026-05-04 ek:** `JobCategory` Faz 1 seed-only kararı; `SystemUser.VerifyEmail` token temizliği; CQRS doctor post-queries + baseline üst notu; Accept/Reject handler `///` özeti; `Program.cs` Faz 1 auth notu; `dotnet format` (yalnız AdaIs `src` projeleri).
- **Takip:** Token **üretimi** (login) uçları; RS256/refresh rotation; RBAC policy; isteğe bağlı **PostgreSQL** skill unique index migration; Swagger UI.

---

## 7. Bilinçli ertelemeler (referans)

| Konu | Ne zaman |
|------|----------|
| Raporlama modülü, export, snapshot | Faz 2 |
| Mutual QR, Redis token, GPS grace, anomali bayrakları | Sprint 2+ |
| pgvector, embedding pipeline, agentic bildirim | Sprint 2+ (PRD Faz 2 hizası) |
| `WorkerPayout` / `CommissionReceivable` tam akış | MVP sonrası (PRD’ye göre sıra) |

---

## Notlar (serbest alan)

- **2026-05-03:** `/ada-is-akademi-plan` ile repo tarandı; yukarıdaki “Repo gerçeği” ve sıra önerisi eklendi.
- **2026-05-03:** Bölüm 1 tamamlandı — PRD fark tablosu (§1.1), ilişki özeti (§1.2), controller kararı (§1.3).
- **2026-05-03:** İlan üretimi **Employer üzerinden**; `Employer.AddJobPosting` + domain hata kodları + `EmployerDomainTests`. Ardından **`CreateJobPostingCommand`** (+ validator, handler, `ServiceRegister`): `JobCategory` doğrulaması handler’da; `Employer` + `Locations` `Include` ile yükleniyor. Sıradaki iş: **HTTP** (`JobPostingsController` veya eşdeğeri).
- **2026-05-03 (devam):** Core `ApiControllerBase` → `ISender` ile gerçek dispatch; `Submit`/`Withdraw` komutları; `JobApplicationConfiguration`; API controller’lar + istek DTO’ları; `IServiceRegister` imza uyumu (Application/Persistence); Core `RedisMessageBus` derleme hatası (lambda parametresi `_` gölgelemesi) düzeltildi.
- **2026-05-03 (plan):** `ada-is-akademi-plan` ile görev dosyası senkronlandı: repo özeti (Model, `ErrorCode`, önbellek), §0/§6 onayları, “Done / follow-ups” ve sıradaki iş maddeleri güncellendi.
- **2026-05-03:** İşveren kapsamı: `ListJobApplicationsByJobPostingIdQuery`, `AcceptJobPostingApplicationCommand`, `RejectJobPostingApplicationCommand` için `EmployerId` + handler eşlemesi; API `employerId` query; `ApplicationValidationCodes` 702/117/118; `JobPostingApplicationValidatorsTests`.
- **2026-05-04:** `JobCategory` Faz 1’de seed-only; `SystemUser.VerifyEmail` başarıda token/süre temizliği; CQRS doctor post-queries + baseline üst notu; `Program.cs` Faz 1 auth notu; Accept/Reject orkestrasyonu + handler `///`; AdaIs `src` projelerinde `dotnet format` (iş bitimlerinde iki tur).
- **2026-05-04:** `src/Api/Models` kaldırıldı; HTTP gövdeleri doğrudan Application komut/sorgu tiplerine (`[FromBody]`). `api-layer.mdc` buna göre güncellendi.
- **2026-05-04 (Faz 1 kapanış):** `Phase1CriticalCommandHandlersTests` + SQLite test host (`SqliteConnectionHolder`, `NullCacheService`, `TestExecutionContext`); `Accept` handler’da `Applications` eager load; `ListJobApplicationsByJobPostingId` sıralama bellekte (SQLite `DateTimeOffset` ORDER BY sınırı); `JobPostingSkill` / `WorkerSkill` EF unique index kaldırıldı (domain + ileride migration SQL); `Persistence` → `Application.Tests` `InternalsVisibleTo`; domain’de aynı worker ikinci `AddApplication` testi.
- **2026-05-04 (Faz 1 tamam):** Core `UseAuthentication` sırası; **JWT Bearer** `JwtConfig` + `AddAzoxiaJwtBearerAuthentication` (Core.Api); AdaIs `appsettings` `JwtConfig`; claim erişimi `IExecutionContext`; controller’larda granular `[AllowAnonymous]`; `ListJobPostingsByEmployerIdQuery` / `JobPostings/ListByEmployer`; `docs/token-rules-faz1.md`; `api-frontend-faz1.md` ve docker-compose `JwtConfig__*` örnek ortamı.
