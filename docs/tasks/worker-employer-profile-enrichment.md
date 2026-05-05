# Worker / Employer profil zenginleştirme ve eşleştirme verisi

İş planına **2026-05** itibarıyla eklenen kapsam. Öncelik sırası ürün güvenliği (işverene kapalı alanlar) ve eşleştirme değerine göre verilmiştir.

## Goal

- Employer için **şirket logosu** (MinIO / worker profil foto ile ortak dosya hattı).
- Worker için **kısa “hakkında” metni**, **sosyal medya linkleri** (platform + URL listesi; domain’de value object / owned JSON).
- Worker için **ilgilendiği pozisyonlar** ve **beklenen maaş aralığı** — **yalnızca worker self + eşleştirme / öneri** tarafında; işveren read modelinden **hariç** tutulur.
- **Profil tamamlanma oranı** — deterministik, dokümante ağırlıklı formül (query veya domain tek noktası).
- **İşveren kaynaklı profil görüntülenme sayısı** — tanım (ör. detay açılışı), kötüye kullanım için sınırlar.
- **Sonraki faz:** AI ile profil analizi — bu dosyada yalnızca *deferred* hatırlatma.

## Out of scope (bu checklist kapanana kadar)

- Mobil/web UI implementasyonu (bu repo’da istemci yok); API + modeller hedeflenir.
- AI profil analizi üretimi (ayrı faz / ayrı onay).

## Öncelik sırası (uygulama sırası)

### P1 öncelik — Employer sosyal linkler (ürün isteği)

- [x] Employer: **kurumsal sosyal / web linkleri** — `EmployerSocialLink` + `SocialMediaPlatform` (worker ile aynı enum); tam liste değişimi `UpdateEmployerSocialLinksCommand`; JWT `employer_id` ile self; `EmployerDetailModel` / `EmployerFullDetailModel` içinde `SocialLinks`; API `Employers/UpdateSocialLinks` (POST); migration `AddEmployerSocialLinks`.

### P0 — Gizlilik + eşleştirme çerçevesi

- [x] Worker “hassas eşleştirme” alanları için **read model ayrımı**: `WorkerSelfDetailModel` / `WorkerSelfFullDetailModel` (worker JWT) vs `WorkerEmployerSafeDetailModel` / `WorkerEmployerSafeFullDetailModel` (işveren JWT + ortak başvuru). Maaş ve ilgi kategorileri işveren uçlarında yok; `Workers/GetById` ve `GetDetail` artık `employer_id` + başvuru bağı gerektirir; worker self: `GetSelfSummary`, `GetSelfFullDetail`.
- [x] Domain: beklenen maaş (ISO 4217 para kodu + min/max `decimal?`), ilgilendiği pozisyonlar `WorkerInterestedJobCategory` → `JobCategory` FK — migration `AddWorkerMatchingPreferences`.
- [x] Application: `UpdateWorkerMatchingPreferencesCommand` + validasyon; `ListSemanticMatchedJobPostingsQuery`: işçinin ilgi kategorileri doluysa yalnızca bu kategorilerdeki açık ilanlar öneriye girer (boş liste = mevcut davranış).

### P0 / P1 — Profil tamamlanma oranı

- [x] Ağırlıkların dokümantasyonu (zekâ sınıfında + bu dosyada tablo; güncel tablo: yetenek 15 · müsaitlik 15 · portfolyo 18 · uyruk/üniversite 8 · maaş iki sınır 13 · ilgi kategorisi 13 · bio 6 · foto 5 · sosyal ≥1 için 7 → 100).
- [x] Hesaplama: `IWorkerProfileCompletionEvaluator` (`WorkerProfileCompletionEvaluator`) + worker self read modellerinde `ProfileCompletionPercent`.
- [x] Cache anahtarları ile tutarlı invalidation (`InvalidateByDependencyAsync(WorkerDependency)` biyografi / sosyal / foto komutlarında; maaş-kategori akışı ile aynı desen).

### P1 — Ortak profil ve medya

- [x] Worker: **About / Bio** (`UpdateWorkerBioCommand`, `Worker.Bio`, EF max 3000).
- [x] Worker: **sosyal medya** — `WorkerSocialLink` + `SocialMediaPlatform`; `UpdateWorkerSocialLinksCommand` (tam liste değişimi).
- [x] Employer: **logo** — `Employer.LogoObjectKey`, `InitEmployerLogoUploadCommand` / `ConfirmEmployerLogoUploadCommand` / `GetEmployerLogoViewUrlQuery`; `IObjectStoragePresigner` (S3 uyumlu + dev stub).
- [x] Employer: **sosyal medya linkleri** — `EmployerSocialLink`; `UpdateEmployerSocialLinksCommand`; özet ve tam detay read modellerinde listelenir.
- [x] Worker: **profil fotoğrafı** — `Worker.ProfilePhotoObjectKey`, init/confirm/view URL komutları (`InitWorkerProfilePhotoUploadCommand` vb.); aynı presigner hattı.

### P2 — Metrikler

- [x] Employer tarafından worker profil **görüntülenme sayısı** — `EmployerWorkerProfileViewStat` (işveren+işçi tekil, UTC takvim günü başına en fazla bir artış), `RecordEmployerWorkerProfileViewCommand` + `Workers/RecordEmployerWorkerProfileView`; işveren güvenli read modellerinde `EmployerSourcedProfileViewCount`; cache anahtarı işveren kimliği ile kapsüllendi ve görüntüleme sayacı için `EmployerWorkerProfileViewStat` bağımlılık etiketi ile invalidation. Ortak başvuru/sayaç okuma: **`IWorkerEmployerProfileAccess`** + `WorkerEmployerProfileAccess` (scoped DI); `GetWorkerById` / `GetDetail` / `RecordEmployerWorkerProfileView` handler’ları ctor’da yalnız `IServiceProvider`, servis çağrıda `GetRequiredService<IWorkerEmployerProfileAccess>()`.

### Deferred (sonraki faz)

- [ ] AI ile profil analizi çıkarımı (prompt, maliyet, KVKK/açık rıza metni, gösterim yüzeyi).

## Bağımlılıklar

- MinIO + `IFileStorage` benzeri soyutlama yoksa önce altyapı; logo ve profil foto aynı servisle beslenir.
- CV pipeline (`CvUploadSession`, PRD §5.3) ile çakışmaz; ayrı epik — `ada-is-akademi-execution-tracker.md` Deferred maddesi.

## Kabul kriterleri (özet)

- İşveren API yanıtlarında maaş / ilgi pozisyonları **asla** yer almaz (regresyon testi veya contract test önerilir).
- Worker kendi profilinde bu alanları okuyup güncelleyebilir.
- Profil tamamlanma % tutarlı ve tekrarlanabilir.
