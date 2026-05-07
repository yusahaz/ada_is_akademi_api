# Application CQRS Doctor Report

## Yönetici Özeti
- Genel risk seviyesi: **High**
- Sistemik CQRS/Application sorunları: Query tarafında tenant kapsamı hatası, bazı use-case'lerde transaction sınırının parçalı kalması, yüksek hacimde in-memory sıralama/paging yapan handler'lar.
- Öncelikli aksiyonlar: Dashboard worker sayısını employer-scope'a çekin, `CreateWorkerPayout` akışını tek transaction/save sınırına alın, semantic/portfolio sorgularını DB tarafında sayfalama ve skorlamaya yaklaştırın.

## Bulgular
### [High] [Correctness/Multi-Tenancy] Dashboard aktif çalışan sayısı tenant dışına taşıyor
- **Neden önemli:** Employer dashboard metrikleri tenant sınırı içinde olmalı; global worker sayısı dönmek iş kuralını ve rapor doğruluğunu bozuyor.
- **Kanıt:** `src/Application/Queries/Employer/GetSpotDashboardSummaryQuery.cs` içinde `activeWorkerCount` sorgusu `Worker` üzerinde sadece `!x.IsDeleted && x.SystemUser.AccountStatus == AccountStatus.Active` filtresi ile çalışıyor; `employerId` filtresi yok.
- **Etkisi:** Yanlış KPI, yanlış operasyon kararı, müşteri güveninde azalma.
- **Öneri:** Aktif çalışan metriklerini employer ilişkili veri üzerinden üretin (ör. ilgili employer’ın `ShiftAssignment`/`JobApplication` geçmişinden distinct worker seti). Buna yönelik doğrulayıcı entegrasyon testi ekleyin.

### [Medium] [Transaction Boundary] `CreateWorkerPayout` iki ayrı `SaveChanges` ile kısmi commit üretebilir
- **Neden önemli:** CQRS command handler tek use-case’in atomik sınırını korumalıdır. İlk save başarılı, ikinci save (audit log) başarısız olursa payout yaratılmış ama audit eksik kalır.
- **Kanıt:** `src/Application/Commands/Employer/CreateWorkerPayoutCommand.cs` içinde önce payout için `SaveChangesAsync`, sonra audit log eklenip tekrar `SaveChangesAsync` çağrılıyor.
- **Etkisi:** Muhasebe izlenebilirliği zayıflar; retry/reconcile süreçleri tutarsız veriyle uğraşır.
- **Öneri:** Payout + audit log eklemelerini tek unit-of-work commit noktasında birleştirin. Teknik gerekçeyle iki faz gerekliyse explicit transaction + compensating action tanımlayın.

### [Medium] [Scalability/CQRS Query Design] Semantic search handler DB dışında tam liste skorlayıp sayfalıyor
- **Neden önemli:** Query use-case'i büyük veri hacminde performans ve latency açısından ölçeklenmeli; tüm adayları belleğe alıp sonra `Skip/Take` yapmak pahalıdır.
- **Kanıt:** `src/Application/Queries/Worker/SemanticSearchWorkersQuery.cs` içinde önce `candidateWorkerIds`, sonra tüm worker listesi çekilip `CalculateSemanticScore` ile bellek içinde sıralanıyor; en sonda `rows.Skip(query.Offset).Take(query.Limit)`.
- **Etkisi:** Yüksek bellek tüketimi, uzun response süreleri, sıcak endpoint'te throughput düşüşü.
- **Öneri:** Aday setini DB tarafında daraltın (örn. full-text/prefix stratejisi, normalize edilmiş arama alanları), mümkünse skorlamayı SQL/projection seviyesine taşıyın ve paging’i veritabanında uygulayın.

### [Medium] [Scalability/CQRS Query Design] Worker portfolio hesaplaması tüm assignment setini bellekte gruplayıp türetiyor
- **Neden önemli:** Read-model query’lerde aggregation mümkün olduğunca veri kaynağına itilmeli; aksi halde worker/assignment büyüdükçe maliyet doğrusal artar.
- **Kanıt:** `src/Application/Queries/Employer/GetWorkerPortfolioQuery.cs` içinde önce worker id listesi, sonra tüm ilgili `ShiftAssignment` kayıtları çekilip `GroupBy` + `Select` ile bellek içi reliability hesaplanıyor.
- **Etkisi:** Büyük employer’larda dashboard ve portföy ekranı gecikmeleri.
- **Öneri:** `completed/noShow/dispute/lastWorkedAt` alanlarını DB aggregate projection ile üretin; mümkünse materialized read-model veya per-employer özet tablosu kullanın.

### [Low] [Validation Consistency] Yeni query validator’ları ApplicationValidationCodes yerine generic core kodu kullanıyor
- **Neden önemli:** Application katmanında use-case’e özgü hata kodları API tüketicisi ve gözlemlenebilirlik için daha anlamlıdır.
- **Kanıt:** `src/Application/Queries/Assignment/ListShiftAssignmentsHistoryQuery.cs`, `src/Application/Queries/Worker/SemanticSearchWorkersQuery.cs`, `src/Application/Queries/Employer/ListWorkerPayoutsQuery.cs` gibi dosyalarda `AzoxiaErrorCodes.RequestValidationFailed.ForField(...)` kullanımı.
- **Etkisi:** Hata sınıflandırması ve istemci tarafı koşullu davranışlar zorlaşır.
- **Öneri:** Bu query’ler için `ApplicationValidationCodes` altında alan-bazlı kodlar tanımlayın; validator’ları buna geçirin.

## CQRS Uygunluk Değerlendirmesi
- Command/Query ayrımı
  - Genel olarak doğru; mutasyonlar command handler’da, read-model üretimi query handler’da.
- Handler sorumluluk dağılımı
  - Çoğu handler orchestration odaklı; ancak bazı query handler’larda (özellikle semantic/portfolio) business-benzeri türetim ve yoğun hesaplama artmış.
- Transaction / UnitOfWork yönetimi
  - Çoğunlukla tek `SaveChanges` akışı var; `CreateWorkerPayout` bu konuda ayrışıyor ve atomiklik riskini artırıyor.
- Validation ve hata stratejisi
  - Girdi doğrulamaları mevcut; fakat bazı yeni query’lerde generic hata koduna kayma var, use-case seviyesinde tutarlılık zayıflıyor.

## Domain İşbirliği Değerlendirmesi
- Domain kuralı sızıntıları
  - Payout lifecycle transition kuralları Domain (`WorkerPayout`) içinde tutulmuş; bu güçlü bir nokta.
  - Query tarafında reliability/dispute gibi türetimler application’da; domain invariant değil ama ölçeklenebilirlik ve tekrar kullanım açısından read-model stratejisine taşınabilir.
- Uygulama katmanında olması gereken vs olmaması gereken kurallar
  - Actor/resource authorization check’leri (`RequireAdaIsEmployerActorId`, `ActorResourceAccessDenied`) application katmanında doğru konumda.
  - Tenant-scope metrik üretimi application policy olarak doğru yerde; ancak implementasyon (global worker count) iş kuralıyla uyumsuz.

## Aksiyon Planı
1. Hızlı kazanımlar
   - `GetSpotDashboardSummaryQuery` içindeki `activeWorkerCount` sorgusunu employer-scope olacak şekilde düzeltin.
   - `CreateWorkerPayoutCommand` için payout + audit log akışını tek commit sınırına indirin.
2. Orta vadeli iyileştirmeler
   - `SemanticSearchWorkersQuery` ve `GetWorkerPortfolioQuery` için DB-first projection/paging tasarımı yapın.
   - Yüksek trafikli read-model’ler için özet tablo/materialized view yaklaşımını değerlendirin.
3. Kalıcı guardrail'ler (testler, checklist, CI kalite kapıları)
   - Tenant isolation testleri: employer dashboard/query sonuçlarında çapraz-tenant veri sızıntısını yakalayan entegrasyon testleri.
   - Transaction bütünlüğü testleri: audit insert fail senaryosunda command atomikliği.
   - Kod inceleme checklist’i: “query’de in-memory paging/sorting var mı?”, “validator use-case kodu dönüyor mu?” maddeleri.
