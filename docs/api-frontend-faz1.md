# Ada İş Akademi — HTTP API rehberi (Faz 1, front-end)

Bu belge **Faz 1** sonrası mobil / web istemcilerinin kullanacağı uçları özetler: **ilan**, **başvuru**, **sistem kullanıcı e-posta doğrulama**. Ürün adı **Ada İş Akademi**’dir.

İlgili iş listesi: `docs/tasks/phase1-identity-jobposting-application.md`.

---

## 1. Genel

| Konu | Açıklama |
|------|-----------|
| **Base URL** | Ortama göre değişir. Yerel geliştirme için örnek: `https://localhost:62498` (`src/Api/Properties/launchSettings.json`). |
| **Biçim** | Tüm uçlar **`application/json`** kullanır. |
| **OpenAPI** | **Development** ortamında OpenAPI belgesi yayınlanır (`Startup` içinde `MapOpenApi`). Şema üretimi ve codegen için çalışan API kökünde OpenAPI endpoint’ini (ör. `/openapi/v1.json`) tarayıcı veya `curl` ile doğrulayın. |
| **CORS** | Core pipeline’da geliştirme dostu geniş politika tanımlıdır; üretimde origin kısıtlaması ayrı yapılandırılacaktır. |
| **Sağlık** | `GET /health` — orchestrasyon / probe. |

---

## 2. Kimlik (Faz 1 gerçeği)

- **`ApiControllerBase`** varsayılan olarak **`[Authorize]`** kullanır. Aşağıdaki istisnalar **`[AllowAnonymous]`** ile açıktır: **`JobPostings/GetById`**, **`JobPostings/ListOpen`**; **`SystemUsers`** e-posta doğrulama uçları.
- **Bearer JWT** (şema: `JwtBearer`): Core `JwtConfig` bölümü (**`JwtConfig:Issuer`**, **`JwtConfig:Audience`**, **`JwtConfig:Key`** — en az 32 karakter) ile HMAC doğrulama; `Azoxia.Core.Api` içinde `AddAzoxiaJwtBearerAuthentication` kayıtlıdır. **Üretimde** anahtarı gizli depo / ortam (`JwtConfig__Key`) ile verin; RS256 geçişi ayrı iş.
- **Claim’ler:** işveren işlemleri için **`employer_id`**, işçi başvuru/çekme için **`worker_id`**. **`IExecutionContext`** üzerinden handler’da okunur; bu rollere göre **JSON gövdesinde `employerId` / `workerId` beklenmez** (istemci yanlışlıkla gönderse bile bağlamda kullanılmaz — modelde alan yoktur).
- **Token üretimi** (login / refresh) bu API paketinin dışında veya sonraki uçlarda; istemciler test için `exp`, `iss`, `aud` doğru olan JWT üretebilir. Detay: `docs/token-rules-faz1.md`.

---

## 3. URL ve HTTP kuralları

`ApiControllerBase` şu şablonu kullanır: **`[controller]/[action]`**.

- **`[controller]`**: Sınıf adından `Controller` soneki düşer, ör. `JobPostingsController` → **`JobPostings`**.
- **`[action]`**: Metot adı, ör. **`Create`**, **`ListOpen`**.

Örnek tam yol (base sonrası):

```text
POST {base}/JobPostings/Create
POST {base}/JobApplications/Submit
PUT  {base}/JobPostings/Update
```

**Önemli:** Okuma işlemleri de (detay, liste) **çoğunlukla `POST` + JSON gövde** ile tanımlıdır; `GET` + query parametreleri kullanılmaz.

---

## 4. Yanıt zarfı (`ApiResponse`)

Başarılı yanıtlar **`Azoxia.Core.Api.Responses`** içindeki zarfla döner.

### 4.1 `ApiResponse` (veri yok / genel)

```json
{
  "success": true,
  "message": null,
  "code": null,
  "data": null,
  "fieldErrors": null
}
```

### 4.2 `ApiResponse<T>` (veri var)

```json
{
  "success": true,
  "message": null,
  "code": null,
  "data": { },
  "fieldErrors": null
}
```

- **`data`**: Komut/query sonucu (ör. oluşturulan ilan `id`, detay modeli, liste).
- **`success: false`**: Hata; **`message`**, isteğe bağlı **`code`**, doğrulama/binding için **`fieldErrors`**.

### 4.3 Alan hatası

`fieldErrors` öğeleri: **`field`**, **`code`**, **`message`** (`ApiFieldError`).

---

## 5. JSON alan adları ve tipler

- Varsayılan ASP.NET Core JSON politikası ile property adları **camelCase** olarak serileştirilir (ör. `employerId`, `jobPostingId`).
- **`DateOnly`**: ISO tarih string (ör. `"2026-05-10"`).
- **`TimeOnly`**: ISO süre string (ör. `"09:00:00"` veya kısa biçim; OpenAPI şemasına bakın).
- **`DateTimeOffset`**: ISO-8601 string.
- **`decimal`**: JSON sayı.
- **Domain enum’ları** (`JobPostingStatus`, `JobApplicationStatus`, …): varsayılan olarak genelde **sayı** olarak gider; kesin sayı/string için **OpenAPI şemasını** kullanın veya örnek yanıt alın.

---

## 6. Uçlar özeti

Aşağıdaki tabloda **gövde**, Application katmanındaki tip adıyla belirtilir; JSON alanları bu tiplerin public property’leri ile aynıdır (camelCase).

### 6.1 İlan — `JobPostings`

| HTTP | Yol | Gövde | Başarı `data` |
|------|-----|--------|----------------|
| POST | `JobPostings/Cancel` | `CancelJobPostingCommand` | — (`ApiResponse`) |
| POST | `JobPostings/Complete` | `CompleteJobPostingCommand` | — |
| POST | `JobPostings/Create` | `CreateJobPostingCommand` | `int` (ilan id) |
| POST | `JobPostings/GetById` | `GetJobPostingByIdQuery` (`jobPostingId`) | `JobPostingDetailModel` |
| POST | `JobPostings/ListByEmployer` | `ListJobPostingsByEmployerIdQuery` (boş `{}` yeterli; kapsam yalnızca JWT `employer_id`) | `JobPostingSummaryModel[]` |
| POST | `JobPostings/ListOpen` | `ListOpenJobPostingsQuery` (opsiyonel; boş `{}` veya gövde yok) | `JobPostingSummaryModel[]` |
| POST | `JobPostings/Publish` | `PublishJobPostingCommand` | — |
| PUT | `JobPostings/Update` | `UpdateJobPostingCommand` | — |

**Not:** `ListOpen` ve `GetById` dışındaki tüm `JobPostings/*` uçları **Bearer JWT** ister; işveren işlemlerinde token’da geçerli **`employer_id`** claim’i zorunludur.

**`CreateJobPostingCommand` alanları (özet):**  
`description`, `employerLocationId`, `headCount`, `jobCategoryId`, `shiftDate`, `shiftStartTime`, `shiftEndTime`, `title`, `wageAmount`, `wageCurrency` — ilgili işveren **`employer_id`** claim’inden türetilir.

**`UpdateJobPostingCommand`:** taslak ilan alanları + **`jobPostingId`**.

**`CancelJobPostingCommand` / `CompleteJobPostingCommand` / `PublishJobPostingCommand`:** tipik olarak **`jobPostingId`**.

**`GetJobPostingByIdQuery`:** **`jobPostingId`**.

**`JobPostingDetailModel` (özet):**  
`id`, `title`, `description`, `status`, `employerId`, `employerLocationId`, `jobCategoryId`, `shiftDate`, `shiftStartTime`, `shiftEndTime`, `wageAmount`, `wageCurrency`, `headCount`, `pendingApplications`, `acceptedApplications`, `skills` (`JobPostingSkillItemModel`: `tag`, `isRequired`).

**`JobPostingSummaryModel`:**  
`id`, `title`, `shiftDate`, `shiftStartTime`, `shiftEndTime`, `wageAmount`, `wageCurrency`, `employerId`, `headCount`.

### 6.2 Başvuru — `JobApplications`

| HTTP | Yol | Gövde | Başarı `data` |
|------|-----|--------|----------------|
| POST | `JobApplications/Accept` | `AcceptJobPostingApplicationCommand` | — |
| POST | `JobApplications/List` | `ListJobApplicationsByJobPostingIdQuery` | `JobApplicationListItemModel[]` |
| POST | `JobApplications/Reject` | `RejectJobPostingApplicationCommand` | — |
| POST | `JobApplications/Submit` | `SubmitJobPostingApplicationCommand` | `int` (başvuru id) |
| POST | `JobApplications/Withdraw` | `WithdrawJobPostingApplicationCommand` | — |

**İşveren doğrulaması:** `Accept`, `Reject`, `List` için JWT **`employer_id`** ilanın sahibi (`JobPosting.EmployerId`) ile eşleşmelidir. Geçersiz / eksik claim **`AZX_ADA_APP_VAL_900`**; sahiplik uyuşmazlığı **`404`** zarfı ile maskelenir.

**`SubmitJobPostingApplicationCommand`:**  
`jobPostingId`, `hasConflictingShift`, `note` (opsiyonel) — başvuran işçi **`worker_id`** claim’inden türetilir.

**`AcceptJobPostingApplicationCommand`:**  
`jobPostingId`, `applicationId`.

**`RejectJobPostingApplicationCommand`:**  
`jobPostingId`, `applicationId`, `reason` (opsiyonel).

**`WithdrawJobPostingApplicationCommand`:**  
`jobPostingId`, `applicationId` — ilgili başvurunun **`worker_id`** claim’i ile aynı işçiye ait olması gerekir (aksi `404`).

**`ListJobApplicationsByJobPostingIdQuery`:**  
`jobPostingId` — işveren **`employer_id`** claim’i ile doğrulanır.

**`JobApplicationListItemModel`:**  
`applicationId`, `workerId`, `status`, `appliedAt`, `note`.

### 6.3 Sistem kullanıcı — `SystemUsers` (e-posta doğrulama)

| HTTP | Yol | Gövde | Başarı `data` |
|------|-----|--------|----------------|
| POST | `SystemUsers/RequestEmailVerification` | `RequestSystemUserEmailVerificationCommand` | — |
| POST | `SystemUsers/VerifyEmail` | `VerifySystemUserEmailCommand` | — |

**`RequestSystemUserEmailVerificationCommand`:**  
`systemUserId`, `tokenHash`, `expiresAt` (`DateTimeOffset`).

**`VerifySystemUserEmailCommand`:**  
`systemUserId`, `tokenHash` (doğrulama isteğinde saklanan ile aynı mantık).

---

## 7. Hata kodları ve HTTP durumları

- **400**: Genelde model binding / doğrulama; gövde **`ApiResponse`** + **`fieldErrors`**.
- **404**: Kaynak veya iş kuralı (ör. ilan sahibi ile `employer_id` uyumsuzluğu, bulunamayan ilan).
- **500**: Beklenmeyen hata; yine **`ApiResponse`** şeklinde global handler ile uyumlu mesaj.

İş kuralı / domain mesajları **`message`** ve **`code`** ile gelir; tam kod listesi için `ApplicationValidationCodes` ve `DomainErrorCodes` kaynak koduna bakılabilir (ileride public hata kataloğu ayrıca yayınlanabilir).

---

## 8. Örnek istekler

### 8.1 Açık ilanları listele

```http
POST /JobPostings/ListOpen
Content-Type: application/json

{}
```

### 8.2 İlan detayı

```http
POST /JobPostings/GetById
Content-Type: application/json

{ "jobPostingId": 1 }
```

### 8.3 Başvuru gönder

`Submit` **kimlik ister**. Bearer JWT’de pozitif **`worker_id`** claim’i gerekli; **`worker_id` gövdede gönderilmez**.

```http
POST /JobApplications/Submit
Authorization: Bearer <jwt>
Content-Type: application/json

{
  "jobPostingId": 1,
  "hasConflictingShift": false,
  "note": null
}
```

### 8.3b İşveren — kendi ilanlarını listele

```http
POST /JobPostings/ListByEmployer
Authorization: Bearer <jwt>
Content-Type: application/json

{}
```

### 8.4 İşveren — başvuruları listele

```http
POST /JobApplications/List
Authorization: Bearer <jwt>
Content-Type: application/json

{
  "jobPostingId": 1
}
```

---

## 9. Swagger UI

Faz 1 host’unda **Swagger UI varsayılan olarak açılmıyor**; OpenAPI belgesi Development’ta üretilir. İstemci ekibi codegen veya Postman import için bu belgeyi kullanmalıdır. İhtiyaç halinde projeye Swagger UI eklenebilir (ayrı iş).

---

## 10. Sürüm notu

Bu doküman **Faz 1** API’sine göre yazılmıştır (JWT doğrulama + `ListByEmployer` dahil). Sonraki değişikliklerde güncellenmelidir.
