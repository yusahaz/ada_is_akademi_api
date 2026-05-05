# Ada İş Akademi - PRD Tüm Fazlar Sprint Planı

## Tracking Anchor
- [x] Aktif ilerleme takibi dosyası oluşturuldu: `docs/tasks/ada-is-akademi-execution-tracker.md`
- [x] Her implementasyon adımı sonrası önce execution tracker güncellenecek, sonra bu plan dosyasındaki ilgili maddeler işaretlenecek.

## Goal
- [x] PRD kapsamındaki Faz 1-2-3 işlerini bağımlılık sırasına göre Sprint 0-9 olarak planlamak.
- [x] Her sprint için net teslim çıktısı, kabul kriteri ve risk azaltma adımlarını takip edilebilir hale getirmek.

## In-scope
- [x] Faz 1 (Core MVP), Faz 2 (Intelligence and Automation), Faz 3 (Scale and Monetization) kapsamlarını sprint backlog'una dönüştürmek.
- [x] Katman sırasına uygun teslim mantığı: Domain -> Application -> Persistence -> API -> Test.
- [x] Teknik ve ürün risklerini sprint bazlı mitigasyonlarla eşlemek.

## Out-of-scope
- [x] Kod implementasyonu, migration çalıştırma, endpoint değişikliği.
- [x] PRD dışı yeni kapsam ekleme.

## Faz Kapsamı ve Bağımlılıklar

### Faz 1 - Core MVP (Sprint 1-3)
- [x] Identity: email verification aktivasyon, JWT, multi-device session, group-based RBAC.
- [x] Worker/Employer çekirdek profilleri ve ilan-başvuru akışı.
- [x] Assignment + mutual QR check-in/out (clock drift + grace period).
- [x] CommissionReceivable + WorkerPayout çekirdeği.
- [x] Bağımlılık: auth + permission resolver tamamlanmadan operasyonel endpointler açılmaz.

### Faz 2 - Intelligence and Automation (Sprint 4-6)
- [ ] pgvector extension, worker/posting embedding pipeline.
- [ ] Semantic matching ve agentic personalized notification + fallback.
- [ ] Hangfire otomasyonları, overdue alarmı, permission cache invalidation.
- [ ] Raporlama query/export akışları (CQRS read-side odaklı).
- [ ] Bağımlılık: Faz 1 domain eventleri ve veri kalitesi olgun olmadan semantic/fallback kalitesi düşük kalır.

### Faz 3 - Scale and Monetization (Sprint 7-9)
- [ ] Employer monetization omurgası (abonelik/kiralama disinda).
- [ ] Çok bölge/çok dil hazırlığı.
- [ ] Assignment + Commission odaklı modüler ayrışma adımı.
- [ ] Outbox/read-replica ve ileri analitik zemini.
- [ ] Bağımlılık: Faz 2 otomasyon ve raporlama olgunluğu olmadan ölçekleme maliyeti artar.

## Sprint Backlog (0-9)

### Sprint 0 - Hazırlık ve Mimari Kararlar
- [ ] PRD iş kırılımı ve faz->sprint haritalama.
- [ ] Cross-cutting kararlar: auth, event, cache, raporlama read-side sınırları.
- [ ] Ortak kabul kriterleri (DoD) ve regresyon kontrol çerçevesi.
- [ ] Çıktı: baseline backlog + risk kaydı + ölçüm metrikleri.

### Sprint 1 - Identity ve Güvenlik Temeli
- [ ] Worker email verification akışı.
- [ ] JWT + refresh rotation + cihaz bazlı session.
- [ ] Group-based RBAC ve permission cache yaklaşımı.
- [x] Çıktı (ilk dikey dilim): kimlik ve yetkilendirme çekirdeği uca açık.

### Sprint 2 - Profil ve İlan-Başvuru Çekirdeği
- [ ] Worker profil bölümleri + CV destekli profil oluşturma akışı.
- [x] Employer profil/lokasyon/supervisor verisi.
- [ ] Job category + job posting + başvuru akışları.
- [x] Çıktı (ilk dikey dilim): vardiya oluşumundan başvuruya temel iş akışının çalışması.

### Sprint 3 - Assignment, QR, Finansal Çekirdek
- [x] Mutual QR check-in/out + anomaly flag mekaniği.
- [x] Commission motoru ve audit log.
- [x] CommissionReceivable oluşturma + WorkerPayout state geçişleri.
- [x] Çıktı (ilk dikey dilim): Faz 1 MVP operasyonel olarak tamam.

### Sprint 4 - Semantic Altyapı
- [x] pgvector ve embedding kolon/indexleri.
- [x] Worker/job embedding üretim triggerları.
- [x] Matching temel sorguları + availability filtreleri.
- [x] Çıktı (ilk dikey dilim): semantic öneri altyapısı çalışır.

### Sprint 5 - Kişiselleştirme ve Gerçek Zamanlılık
- [x] Agentic personalized notification.
- [x] LLM fallback mekanizması ve kanal geçişi.
- [ ] Canlı durum akışları (matching/assignment bildirimi).
- [ ] Çıktı: kişisel ve zamanında iletişim deneyimi.
- [x] Uygulanan dikey dilim: worker notification preview query + push->email fallback + API endpoint + test eklendi.
- [x] Uygulanan dikey dilim: notification preview semantic personalization score/source + push->email->in_app fallback zinciri + test eklendi.

### Sprint 6 - Otomasyon ve Raporlama
- [ ] Hangfire scheduler: faturalama/periyodik işler.
- [ ] Docker backup reposu: `postgres:17-alpine` ve MinIO datalari için periyodik yedekleme akışını (schedule + retention + restore smoke) kur.
- [ ] Overdue alarm mekaniği.
- [ ] CQRS rapor query/export paketleri.
- [ ] Çıktı: Faz 2 operasyonel otomasyon + raporlama tabanı tamam.
- [x] Uygulanan dikey dilim: overdue summary query + statistics endpoint + cache + test eklendi.
- [x] Uygulanan dikey dilim: Hangfire recurring job + idempotent overdue alarm sweep command + migration + test eklendi.
- [x] Uygulanan dikey dilim: overdue alarm CSV export query + statistics endpoint + cache + test eklendi.

### Sprint 7 - Monetization Başlangıcı
- [x] Uygulanan dikey dilim: monetization summary query + statistics endpoint + cache + test eklendi.
- [x] Uygulanan dikey dilim: employer commission policy command/query + employer endpointleri + cache + test eklendi.
- [x] Uygulanan dikey dilim: employer commission policy CSV export query + employer endpoint + cache + test eklendi.
- [x] Uygulanan dikey dilim: employer commission estimate query + employer endpoint + cache + test eklendi.
- [x] Uygulanan dikey dilim: employer commission summary list query + employer endpoint + cache + test eklendi.
- [x] Uygulanan dikey dilim: commission receivable period-idempotent generation command + employer endpoint + query + cache + test eklendi.
- [x] Uygulanan dikey dilim: commission receivable list query + employer endpoint + cache + test eklendi.
- [x] Uygulanan dikey dilim: employer/worker/systemusergroup/systemuser filtreli liste query + endpoint + validator testleri eklendi.
- [x] Uygulanan dikey dilim: jobposting/jobapplication/commission receivable liste endpointleri `PageableApiResponse` formatina gecirilerek sayfalama metadatasi standartlastirildi.

### Sprint 8 - Coğrafi ve Üretim Ölçeği
- [ ] Çok bölge/çok dil hazırlık backlog'u.
- [ ] Ölçek dayanımı ve operasyonel guardrail'ler.
- [ ] Çıktı: büyüme senaryoları için teknik hazırlık.

### Sprint 9 - Modüler Ayrışma ve Dayanıklılık
- [ ] Assignment/Commission modüllerinde ayrışma adımı.
- [ ] Outbox pattern geçiş stratejisi.
- [ ] Read-replica/ileri analitik yol haritası.
- [ ] Çıktı: Faz 3 kapanışı ve sonraki genişleme temelinin netleşmesi.

## Sprintlere Ayrılmış Teknik İş Kırılımı

### Sprint 0 - Hazırlık ve Mimari
- [ ] Domain: Faz bazlı aggregate sınırlarını ve invariants listesini netleştir.
- [ ] Application: CQRS isimlendirme ve klasörleme şablonunu sabitle.
- [ ] Persistence: Migration stratejisi (idempotent seed, rollback notu) dokümante et.
- [ ] API: Endpoint sözleşmesi (ApiResponse envelope, auth matrix) kontrol listesi çıkar.
- [ ] Test: Sprint bazlı smoke/regresyon matrisi tanımla.

### Sprint 1 - Identity ve Güvenlik
- [ ] Domain: Email verification token, session ve group/permission kurallarını finalize et.
- [ ] Application: Register/Verify/Login/Refresh/Logout komut-sorgu akışlarını validator + handler ile tamamla.
- [ ] Persistence: SystemUser, SystemUserGroup ve token/session mapping + indeksleri tamamla.
- [ ] API: Auth ve system user endpointlerini body-only CQRS sözleşmesiyle aç.
- [ ] Test: Email verify bypass, refresh replay, permission cache invalidation senaryolarını doğrula.

#### Sprint 1 Mikro Görevler (Gerçek Dosya/Path Bazlı)
- [ ] Domain - `src/Domain/Entities/SystemUser.cs`: `RequestEmailVerification`, `VerifyEmail`, `RecordFailedLoginAttempt`, `RevokeAllRefreshTokens` akışlarında kural/kenar durumlarını gözden geçir ve eksik iş kuralı varsa ekle.
- [ ] Domain - `src/Domain/Entities/SystemUserRefreshToken.cs`: `IsActive`, `IsExpired`, `Revoke`, `Until` davranışlarını refresh rotation senaryosuna göre doğrula.
- [ ] Domain - `src/Domain/Entities/SystemUserGroup.cs`: `AddPermission`, `Activate`, `Deactivate` davranışlarını RBAC beklentileriyle hizala.
- [ ] Application - `src/Application/Commands/SystemUser/RegisterAdminCommand.cs`, `src/Application/Commands/SystemUser/RegisterEmployerCommand.cs`, `src/Application/Commands/SystemUser/RegisterWorkerCommand.cs`: kayıt akışlarında validator/handler bütünlüğünü ve dönen kimlik sözleşmesini doğrula.
- [ ] Application - `src/Application/Commands/SystemUser/LoginSystemUserCommand.cs`: device-bound login, failed-attempt lock ve claim üretimini netleştir.
- [ ] Application - `src/Application/Commands/SystemUser/RefreshSystemUserTokenCommand.cs`: refresh token rotation, device eşleşmesi ve eski token revoke adımlarını doğrula.
- [ ] Application - `src/Application/Commands/SystemUser/RequestSystemUserEmailVerificationCommand.cs`, `src/Application/Commands/SystemUser/VerifySystemUserEmailCommand.cs`: token hash + expiry + aktivasyon akışını doğrula.
- [ ] Application - `src/Application/Commands/Authorization/ActivateSystemUserGroupCommand.cs`, `src/Application/Commands/Authorization/DeactivateSystemUserGroupCommand.cs`, `src/Application/Commands/Authorization/AddSystemUserGroupPermissionCommand.cs`: group-based RBAC command setini tamamla ve tutarlılığı kontrol et.
- [ ] Application - `src/Application/DependencyInjection/ServiceRegister.cs`: Sprint 1 kapsamındaki command/query/validator registrationlarının eksiksiz olduğundan emin ol.
- [ ] Persistence - `src/Persistence/Mapping/SystemUserConfiguration.cs`, `src/Persistence/Mapping/SystemUserDeviceConfiguration.cs`, `src/Persistence/Mapping/SystemUserRefreshTokenConfiguration.cs`: authentication/session alanları ve indekslerin performans/tutarlılık uygunluğunu doğrula.
- [ ] Persistence - `src/Persistence/Mapping/SystemUserGroupConfiguration.cs`, `src/Persistence/Mapping/SystemUserGroupMembershipConfiguration.cs`, `src/Persistence/Mapping/SystemUserGroupPermissionConfiguration.cs`: grup-üyelik-izin ilişkilerini ve FK yapılarını doğrula.
- [ ] Persistence - `src/Persistence/Migrations/20260504085518_Initial_v1.cs`, `src/Persistence/Migrations/20260504120000_SeedSystemUserGroupAndDefaultAdmin.cs`, `src/Persistence/Migrations/AdaIsAkademiDbContextModelSnapshot.cs`: seed + migration bütünlüğünü ve geri alma etkisini gözden geçir.
- [ ] API - `src/Api/Controllers/SystemUsersController.cs`: register/login/refresh/verify actionlarının ApiResponse envelope + body-only CQRS sözleşmesini doğrula.
- [ ] API - `src/Api/Controllers/SystemUserGroupsController.cs`: activate/deactivate/add-permission aksiyonlarının RBAC yönetim beklentilerini karşıladığını doğrula.
- [ ] Test - `tests/DomainTests/SystemUserDomainTests.cs`: email verification, lock, refresh revoke domain kural testlerini genişlet.
- [ ] Test - `tests/ApplicationTests/SystemUserIdentityValidatorsTests.cs`: Sprint 1 validator senaryolarına (boş alanlar, geçersiz id, expiry geçmiş) ek testler yaz.
- [ ] Test - `tests/ApplicationTests/` altında yeni test dosyaları: login/refresh/verify handler davranış testlerini ekle (örn. `SystemUserAuthCommandHandlersTests.cs`).
- [x] Uygulanan dikey dilim: `LogoutSystemUserCommand` + `SystemUsersController.Logout` + DI registration + validator testi eklendi.

### Sprint 2 - Profil ve İlan-Başvuru Çekirdeği
- [ ] Domain: Worker/Employer profile ve JobPosting/JobApplication yaşam döngülerini netleştir.
- [ ] Application: Profil yönetimi, ilan oluşturma/yayınlama ve başvuru komut-sorgularını tamamla.
- [ ] Persistence: Profil ve ilan/başvuru entity mapping, relation ve status indekslerini ekle.
- [ ] API: Profil, ilan ve başvuru controller aksiyonlarını sözleşmeye uygun yayımla.
- [ ] Test: Profil eksik veri, duplicate başvuru, ilan statü geçiş regresyonlarını çalıştır.
- [x] Uygulanan dikey dilim: `WithdrawJobPostingApplicationCommandHandler` için owner-worker withdraw senaryosu handler testi eklendi.

### Sprint 3 - Assignment, QR ve Finansal Çekirdek
- [ ] Domain: Assignment check-in/out kuralları, anomaly flag ve payout state transitionlarını finalize et.
- [ ] Application: QR doğrulama, assignment tamamlama ve payout tetikleyici command akışlarını tamamla.
- [ ] Persistence: Assignment, check event ve finansal çekirdek tabloları/mappinglerini tamamla.
- [ ] API: Assignment operasyon ve finansal özet endpointlerini ekle.
- [ ] Test: QR replay, race condition ve payout idempotency senaryolarını doğrula.
- [x] Uygulanan dikey dilim: `ShiftAssignment` çekirdeği + create/check-in command/endpoint + handler testleri eklendi.

### Sprint 4 - Semantic Altyapı
- [ ] Domain: Matching skoru için kuralları ve minimum eşleşme eşiğini tanımla.
- [ ] Application: Embedding üretim ve semantic matching query pipeline'ını ekle.
- [ ] Persistence: pgvector kolonları, index ve update stratejisini migration ile uygula.
- [ ] API: Matching preview/query endpointlerini performans sınırıyla aç.
- [ ] Test: Stale embedding ve fallback keyword arama tutarlılığını ölç.
- [x] Uygulanan dikey dilim: `ListSemanticMatchedJobPostingsQuery` + API endpoint + validator testi eklendi.

### Sprint 5 - Kişiselleştirme ve Gerçek Zamanlılık
- [ ] Domain: Bildirim öncelik ve kanal fallback kurallarını tanımla.
- [ ] Application: Agentic personalized notification orkestrasyonu + fallback akışlarını tamamla.
- [ ] Persistence: Notification log, delivery status ve retry metadata saklamasını ekle.
- [ ] API: Bildirim tercihleri ve canlı durum besleme endpointlerini aç.
- [ ] Test: LLM servis kesintisi ve gecikmeli teslimat fallback senaryolarını doğrula.

### Sprint 6 - Otomasyon ve Raporlama
- [ ] Domain: Overdue ve period-close iş kurallarını netleştir.
- [ ] Application: Hangfire job handlerları ve CQRS rapor query/export akışlarını tamamla.
- [ ] Persistence: Scheduler lock/idempotency tabloları ve raporlama read model optimizasyonunu uygula.
- [ ] API: Rapor endpointleri ve export aksiyonlarını güvenli erişimle aç.
- [ ] Test: Duplicate period, job retry ve rapor doğruluk regresyonlarını doğrula.

### Sprint 7 - Monetization Başlangıcı
- [ ] Domain: Abonelik planı, faturalama periyodu ve yetki paketleme kurallarını ekle.
- [ ] Application: Plan atama, yükseltme/düşürme ve fatura üretim komutlarını tamamla.
- [ ] Persistence: Billing/commission entity mapping ve unique constraintleri migration ile uygula.
- [ ] API: Abonelik yönetimi ve fatura görüntüleme endpointlerini yayımla.
- [ ] Test: Plan geçişi, period çakışması ve fatura tutarlılığını doğrula.

### Sprint 8 - Çok Bölge / Çok Dil ve Ölçek
- [ ] Domain: Bölgeye bağlı kural farklılıklarını ve i18n metin sahipliğini belirle.
- [ ] Application: Bölge/dil bazlı query filtreleme ve feature flag uygulamasını tamamla.
- [ ] Persistence: Bölgesel partitioning/read model ve locale metadata alanlarını ekle.
- [ ] API: Dil-bölge parametrelerini CQRS body sözleşmesine entegre et.
- [ ] Test: Bölge-dil kombinasyonlarında kontrat ve performans regresyonlarını çalıştır.

### Sprint 9 - Modüler Ayrışma ve Dayanıklılık
- [ ] Domain: Assignment ve Commission bounded context sınırlarını kesinleştir.
- [ ] Application: Ayrıştırılmış modül command/query orkestrasyonunu ve geçiş adaptörlerini tamamla.
- [ ] Persistence: Outbox/read-replica geçiş adımları ve veri tutarlılık kontrollerini uygula.
- [ ] API: Modülleşme sonrası endpoint geriye uyumluluk katmanını doğrula.
- [ ] Test: Event kaybı, eventual consistency gecikmesi ve failover senaryolarını doğrula.

## Sprint Definition of Done (Tüm Sprintler)
- [ ] Sprint kapsamındaki dikey dilimler mevcut API kontratlarını bozmadan çalışır.
- [ ] CQRS düzeni korunur: command/query + validator + handler uyumu.
- [ ] Gerekli DI kayıtları ve servis bağlantıları doğrulanır.
- [ ] En az kritik path testleri ve regresyon kontrolleri tamamlanır.
- [ ] Ertelenen maddeler açıkça "deferred" etiketiyle backlog'a yazılır.

## Sprint Bazlı Risk Matrisi ve Mitigasyon
- [ ] Sprint 1 riski: email verification bypass -> token hash + tek kullanım + TTL kontrolü.
- [ ] Sprint 2 riski: veri kalitesi düşük profil -> CV import review + zorunlu alan tamamlama.
- [ ] Sprint 3 riski: QR replay / payout race -> Redis GETDEL + pessimistic lock önceliği.
- [ ] Sprint 4 riski: stale embedding -> event-driven regeneration + keyword fallback.
- [ ] Sprint 5 riski: LLM kesintisi -> template fallback ile bildirim gecikmeden devam.
- [ ] Sprint 6 riski: duplicate invoice period -> unique constraint + idempotent scheduler.
- [ ] Sprint 7 riski: monetization kural karmasasi -> kapsamli policy ve migration denetimi.
- [ ] Sprint 8 riski: bölge-dil regresyonu -> feature flag + kademeli rollout.
- [ ] Sprint 9 riski: dağıtık event kaybı -> outbox + teslimat gözlemi.

## Done / Follow-ups
- [x] Plan dosyası güncellendi ve Ada İş Akademi adlandırmasına normalize edildi.
- [x] Implementasyon sırasında her sprint kapanışında tamamlanan maddeler işaretlenecek.
- [x] Faz dışına taşan istekler yeni satır olarak "deferred" etiketiyle eklenecek.
- [ ] deferred: Sprint 7 icin commission policy degisiklik gecmisi (audit trail) query + endpoint + test.
- [ ] deferred: Sprint 1/2 auth-query performans uyarıları için `QuerySplittingBehavior` kararı ve optimize düzenleme.
- [ ] deferred: DataProtection key encryption (container volume var; encryptor policy production için ayrıca netleşecek).
