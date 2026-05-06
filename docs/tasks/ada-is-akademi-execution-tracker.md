# Ada İş Akademi - Execution Tracker

Bu dosya, her iş tamamlandığında güncellenen **tek kaynak** takip alanıdır.
`/ada-is-akademi-plan` çalıştırıldığında önce bu dosya okunur ve plan kaldığı yerden devam ettirilir.

## Kullanım Kuralı
- Her tamamlanan işten sonra yalnızca ilgili satırları güncelle.
- Yeni kapsam eklenirse `Deferred Backlog` bölümüne ekle.
- Faz geçişlerinde `Current Phase` ve `Current Task` alanlarını birlikte güncelle.
- Her güncellemede `Last Updated` alanına tarih/saat yaz.

## Current Status
- Last Updated: 2026-05-06 (UTC+3)
- Current Phase: Faz 2 (profil zenginleştirme checklist kapalı)
- Current Task: —
- Next Task: Finansal mutabakat phase 2: filtreli (employer/date/status) reconciliation raporu + export.
- Blockers: Yok

## Phase Checklist

### P0 - Stabilizasyon ve Güvenlik Sertleştirme
- [x] Refresh/Logout akışında body userId yerine claim/token bağlama
- [x] Kritik endpointlerde policy/claim enforcement netleştirme
- [x] Submit başvuru çakışma kontrolünü server-side doğrulamaya taşıma
- [x] Accept akışında concurrency/kapasite guard stratejisi

### P1 - İşveren/İlan Tutarlılığı
- [x] JobPosting update/add-skill/remove-skill cache invalidation standardizasyonu
- [x] AddSkill endpoint açıklaması ve davranış hizalama
- [x] Açık ilan sorgularında soft-delete filtre standardizasyonu
- [x] ListOpen için cache stratejisi kararı ve uygulama

### P2 - Başvuru/Assignment Genişletme
- [x] Application lifecycle için Expired durum ihtiyacı kararı
- [x] Assignment check-out akışının eklenmesi
- [x] Worker odaklı query/endpointler (başvurularım, assignment geçmişi)
- [x] Accept sonrası assignment üretimi için operasyonel güvenilirlik adımı

## Work Log
- [x] 2026-05-05 15:54 - Kalan işler repo analizi tamamlandı, P0/P1/P2 sıralaması netleştirildi.
- [x] 2026-05-05 16:08 - P0 tamamlandı: refresh/logout token-device tabanlı sertleştirme, claim bağlama kontrolü, submit conflict server-side hesaplama.
- [x] 2026-05-05 16:08 - P1 tamamlandı: job posting command cache invalidation genişletildi, AddSkill endpoint sözleşmesi netleştirildi, ListOpen/ListSemanticMatched soft-delete ve cache standardize edildi.
- [x] 2026-05-05 16:08 - P2 tamamlandı: `Expired` durumu eklendi, check-out command+endpoint eklendi, worker self list query/endpointleri eklendi.
- [x] 2026-05-05 16:36 - Hızlı kapanış: `dotnet test` (Domain+Application) tamamı geçti; Docker API smoke logları kritik hata olmadan doğrulandı.
- [x] 2026-05-05 16:36 - Runtime hardening: `krb5-libs`, DataProtection volume ve izin düzeltmesi uygulandı.
- [x] 2026-05-05 17:03 - Sprint 2 profil derinleştirme: `AddEmployerLocation`, `AddEmployerSupervisor`, `RemoveEmployerSupervisor`, `DeleteEmployer` endpoint/command akışları ve DI kayıtları eklendi; build+linter temiz.
- [x] 2026-05-05 17:07 - Sprint 2 doğrulama: employer profil/supervisor/delete command handler testleri eklendi; ilişkili kullanıcı soft-delete akışı testle doğrulandı.
- [x] 2026-05-05 17:21 - Sprint 2 doğrulama: worker profile command handler testleri (availability add/remove + delete worker soft-delete) eklendi ve geçti.
- [x] 2026-05-05 17:22 - Sprint 3 doğrulama: assignment handler testlerine check-out success ve non-owner check-in access denied senaryoları eklendi.
- [x] 2026-05-05 17:24 - Sprint 3 anomaly guard: check-out sonrası erken çıkışta assignment anomaly işaretleme (`EARLY_CHECKOUT`) ve worker assignment listesine anomaly alanları eklendi; test+build geçti.
- [x] 2026-05-05 17:28 - Sprint 4 semantic guard: stale worker embedding durumunda semantic listede fallback (open postings, score=0) eklendi; query testi + build geçti.
- [x] 2026-05-05 17:29 - Sprint 5 metadata: personalized notification preview modeline `FallbackReason` ve `GeneratedAtUtc` alanları eklendi; query testi + build geçti.
- [x] 2026-05-05 17:30 - Sprint 6 guard: overdue alarm sweep komutunda soft-deleted ilanlar hariç tutuldu; idempotency testi bu senaryoyu kapsayacak şekilde güncellendi.
- [x] 2026-05-05 17:34 - Sprint 7 guard: commission receivable generation komutuna employer `Active` status zorunluluğu eklendi; non-active employer negatif testi yazıldı.
- [x] 2026-05-05 17:36 - Sprint 8 regional: `ListOpenJobPostingsQuery` için opsiyonel `CountryCode` filtresi ve cache key varyantı eklendi; query testi + build geçti.
- [x] 2026-05-05 17:37 - Sprint 9 kapanış: faz boyunca tamamlanan sprint dilimleri tracker/checklist ile senkronize edildi, kapanış commit zinciri tamamlandı.
- [x] 2026-05-05 17:34 - Sprint 7 guard: commission receivable generation komutuna employer `Active` status zorunluluğu eklendi; non-active employer negatif testi yazıldı.
- [x] 2026-05-05 17:56 - Sprint 3 mutual QR: `ShiftAssignment` için worker + supervisor çift token doğrulama, grace-period guard, yeni supervisor check-in command/endpoint ve negatif erişim testi eklendi; sprint testleri geçti.
- [x] 2026-05-05 23:05 - Sprint 3 finansal çekirdek: `WorkerPayout` entity + status transition komutları (create/mark processing/fail/retry/confirm), `CommissionAuditLog` append-only kayıtları, ilgili API endpointleri/DI/migration ve payout+commission testleri tamamlandı.
- [x] 2026-05-05 23:18 - Sprint 4 semantic altyapı: `RunEmbeddingRefreshSweepCommand` + saatlik Hangfire işi + deterministic vectorizer eklendi, `EnablePgvectorExtension` migrationı yazıldı, semantic matching query worker availability filtresiyle güçlendirildi ve Sprint 4 testleri geçti.
- [x] 2026-05-05 23:26 - Sprint 5 kişiselleştirme: notification preview modeline `PersonalizationScore` + `PersonalizationSource` eklendi; semantic cosine hesaplama, template varyantı ve `push -> email -> in_app` fallback zinciri handler’da uygulandı; Sprint 5 query testleri güncellendi/genişletildi.
- [x] 2026-05-06 00:21 - Sprint 5 canlı durum: `GetWorkerLiveStatusFeedQuery` + validator/handler + worker endpoint eklendi; assignment_status ve matching_update polling feed modeli oluşturuldu, testle doğrulandı.
- [x] 2026-05-06 00:37 - Sprint 6 bildirim teslimatı: `SystemUserNotificationDispatch` outbox entity/mapping/migration, `SendWorkerNotificationCommand` + `SendSystemUserNotificationCommand` (push->email->in_app fallback), `RetryFailedSystemUserNotificationsCommand` + 10dk Hangfire job, worker/system-user notification endpointleri ve dispatch testleri tamamlandı.
- [x] 2026-05-06 01:00 - Sprint 6 Docker backup: `docker-compose` icine backup servisi eklendi; Postgres dump + MinIO arsiv + retention + restore smoke akisi `docker/backup/*` altinda tamamlandi.
- [x] 2026-05-06 01:07 - Sprint 6 raporlama: `ExportSystemUserNotificationDispatchesCsvQuery` + `StatisticsController` export endpointi + cache + test eklendi.
- [x] 2026-05-06 - Profil P0: işçi maaş beklentisi + ilgi `JobCategory` listesi (domain + migration), `WorkerEmployerSafe*` vs `WorkerSelf*` read modelleri, işveren görünümü yalnız ortak başvuru ile, `UpdateWorkerMatchingPreferencesCommand`, semantic eşleştirmede kategori filtresi, ApplicationTests + görev dosyası güncellendi.
- [x] 2026-05-06 - Profil P1: worker `Bio` + `WorkerSocialLink`, `ProfilePhotoObjectKey`; employer `LogoObjectKey`; EF migration `AddWorkerProfileBioSocialAndMediaKeys`; `IObjectStoragePresigner` + MinIO/S3 uyumlu `AwsS3CompatibleObjectStoragePresigner` ve dev stub; API uçları (`UpdateBio`, sosyal liste, foto init/confirm/view, logo init/confirm/view); profil tamamlanma ağırlıkları 100’e rebalance; Application + Domain testleri yeşil.
- [x] 2026-05-06 - Employer sosyal linkler: `EmployerSocialLink` + `EmployerSocialLinkInput`, `ReplaceSocialLinks`, migration `AddEmployerSocialLinks`, `UpdateEmployerSocialLinksCommand` + validator/handler/DI, `EmployerDetailModel` / `EmployerFullDetailModel` `SocialLinks`, `Employers/UpdateSocialLinks`, ApplicationTests; PRD v6.2 ve iş planı güncellendi.
- [x] 2026-05-06 - Profil P2 metrik: `EmployerWorkerProfileViewStat` + migration, `IWorkerEmployerProfileAccess` / `WorkerEmployerProfileAccess` (scoped DI), employer-safe cache anahtarları (`employerId`+`workerId`) ve `EmployerWorkerProfileViewStatDependency`, `EmployerSourcedProfileViewCount` read model alanı, `RecordEmployerWorkerProfileViewCommand` + `Workers/RecordEmployerWorkerProfileView`, ApplicationTests + görev dosyası güncellendi.
- [x] 2026-05-06 - Profil P2 teknik: işveren-worker erişim servisi static değil; ilgili handler’lar ctor’da yalnız `IServiceProvider`, `IWorkerEmployerProfileAccess` `ServiceProvider.GetRequiredService` ile çözülüyor (`GetWorkerById` / `GetDetail` / `RecordEmployerWorkerProfileView`).
- [x] 2026-05-07 - Mimari envanter: `/ada-is-akademi-plan` kapsamında katman kurallarıyla hizalı kod yapısı gözden geçirildi; özet ve takip maddeleri `docs/tasks/codebase-structure-review-2026-05.md` dosyasına işlendi.
- [x] 2026-05-07 - Görev klasörü: `docs/tasks/README.md` eklendi (kebab-case adlandırma, tracker ilişkisi, ne zaman yeni task dosyası açılır).
- [x] 2026-05-07 - Domain: beklenen maaş kolonlarından `Money` örneklemesi `Worker.GetExpectedSalaryMinMoney` / `GetExpectedSalaryMaxMoney` olarak taşındı; Application `WorkerExpectedSalaryMappings` kaldırıldı; gözden geçirme task checklist güncellendi.
- [x] 2026-05-06 - Teknik borç: Core `IEntityFilterContext.AsSplitQuery`; Application worker/employer/job posting ve SystemUser auth akışlarında çoklu Include + split query; API DataProtection `SetApplicationName` + opsiyonel PKCS12 `ProtectKeysWithCertificate`; `deployment.md` / `appsettings`; agent workflow ezber yasağı netligi.
- [x] 2026-05-06 - CV pipeline phase 1: `CvUploadSession` domain (status + format enum + lifecycle guard), persistence mapping+migration (`20260506060812_AddCvUploadSession`), worker API komutları (`InitWorkerCvUploadCommand`, `ConfirmWorkerCvUploadCommand`) ve `Workers` controller endpointleri eklendi; build + ApplicationTests geçti.
- [x] 2026-05-06 - CV pipeline phase 2: extraction sweep komutu (`RunCvExtractionSweepCommand`) + Hangfire recurring job (`cv-extraction-sweep`) + `ICvExtractionService`/`FakeCvExtractionService`; worker review komutları (`ConfirmWorkerCvReviewCommand`, `DiscardWorkerCvReviewCommand`) ve endpointleri eklendi; build + ApplicationTests geçti.
- [x] 2026-05-06 - CV pipeline phase 3: `ConfirmWorkerCvReviewCommand` payload apply (education/experience/certificate/language/skill) + granular seçim bayrakları (`Apply*`), worker cache invalidation; extraction payload parser eklendi; build + ApplicationTests geçti.
- [x] 2026-05-06 - Finansal mutabakat phase 1: `GetFinancialReconciliationSummaryQuery` + statistics endpoint eklendi (receivable/payout status sayıları + para birimi bazlı tutarlar), cache key/dependency bağlandı; build + ApplicationTests geçti.
- [x] 2026-05-06 - Hata modeli + maaş okuma: BCL `ArgumentNullException`/`ArgumentException` kaldırıldı; `GuardExtensions` + `AzoxiaErrorCodes`/`DomainErrorCodes`. Worker üzerinde beklenen maaş `Money` projeksiyonu entity’den çıkarıldı (kurallar: entity’de `public` instance metot yok); `GetWorkerSelfDetail` / `GetWorkerSelfFullDetail` handler’larında `MapWorkerExpectedSalary`; profil tamamlanma maaş kontrolü skalar alanlarla. `WorkerExpectedSalaryProjection` DI sınıfı yok. Katman `.cursor/rules` güncellendi. Commit `f199dbc` push `main`.

## Profil / medya / eşleştirme (iş planı özeti)

Detaylı checklist: **`docs/tasks/worker-employer-profile-enrichment.md`**

- **P0:** İşverene kapalı alanlar (maaş aralığı, ilgi pozisyonları) + read model ayrımı; ardından alanların domain/API ekleri.
- **P0–P1:** Profil tamamlanma oranı (deterministik formül).
- **P1:** Worker hakkında (bio), sosyal link VO listesi, employer şirket logosu, employer kurumsal sosyal link listesi, worker profil foto (MinIO — aşağıdaki madde ile birleşik hat).
- **P2:** İşveren kaynaklı profil görüntülenme sayısı.
- **Sonraki faz:** AI profil analizi (deferred — üstteki task dosyasında).

## Deferred Backlog
- [x] **(Öncelik — ürün)** Worker profil fotoğrafı + employer logo: API’de presigned PUT init + confirm + presigned GET görüntüleme; kalıcı object key alanları (`Worker.ProfilePhotoObjectKey`, `Employer.LogoObjectKey`); `IObjectStoragePresigner` ile MinIO/S3 uçları (`ObjectStorageConfig` — Azoxia Core `IConfig` bölümü). *İstemci Flutter/web bu repoda yok; URL sözleşmesi `Workers` / `Employers` controller açıklamalarında.*
- [x] **(Ürün)** CV yükleme + çıkarma pipeline (PRD §5.3): phase 1-3 tamam (upload session + init/confirm API + extraction sweep + review confirm/discard + confirmed payload apply).
- [ ] Finansal mutabakat ve ileri raporlama detayları (phase 1 tamam: reconciliation summary endpoint; kalan: filtreli rapor + export)
- [x] Query splitting/performance warning cleanup (EF `AsSplitQuery` repository akışı + çoklu Include sorguları)
- [x] DataProtection key encryption policy (`SetApplicationName`, opsiyonel PKCS#12 ile `ProtectKeysWithCertificate`, deployment notu)
- [ ] Sprint 8: çok bölge / çok dil hazırlık backlog'u (bilinçli erteleme — talep gelince açılacak)

## Session Handoff Template
Her yeni oturum başında bu blok güncellenir:

- Handoff Summary: CV pipeline phase 1-3 tamamlandı; finansal mutabakat için phase 1 reconciliation summary endpoint eklendi.
- Nerede kaldık: Finansal mutabakat phase 2 (filter/export) ve sprint-8 çok bölge hazırlığı bekliyor.
- Bir sonraki tek adım: report filter sözleşmesiyle `ListFinancialReconciliationRowsQuery` + CSV export.
- Risk/Not: CV extraction servisi deterministik placeholder (`FakeCvExtractionService`); gerçek AI extractor ayrı hardening işi.
