# Application CQRS Doctor Report

**Son gözden geçirme:** 2026-05-04 (Ada Is Akademi `src` + `tests`; Core hariç.)

## Yönetici Özeti

- **Genel risk seviyesi:** Medium
- **Sistemik CQRS/Application sorunları:** Komut ve sorgu işleyicileri `Application.DependencyInjection.ServiceRegister` içinde **manuel** `IRequestHandler<,>` ile kayıtlı; okuma tarafında birden fazla sorgu mevcut. Host tarafında `AddAzoxiaCore`, üretilen `GeneratedServiceRegistrar` ile `Azoxia.AdaIsAkademi.Application.DependencyInjection.ServiceRegister` ve `Persistence` kayıtlarını (ayrıca Core `IServiceRegister` örneklerini) **sırayla** çağırır — **manuel Api `ServiceRegister` dosyası kullanılmamalı** (çift `Register` riski). **`tests/ApplicationTests`** altında `AdaIsCacheKeysTests` ve `JobPostingApplicationValidatorsTests` (6 test) mevcut; handler/entegrasyon senaryoları hâlâ sınırlı.
- **Öncelikli aksiyonlar:** Yeni komut/sorgu eklerken `Application.ServiceRegister` listesini güncelleme disiplini (PR/CI); handler veya entegrasyon testleri; `ListOpenJobPostings` EF projeksiyonunun provider ile izlenmesi; JWT sonrası işveren kimliğinin query parametresi yerine oturumdan beslenmesi.

## Bulgular

### [Low] [DI / Host] Kayıt zinciri üretilen `AddAzoxiaCore` içinde — çift `Register` tuzağı

- **Neden önemli:** `DependencyInjection` üreteci, keşfedilen her `IServiceRegister` için `Register` çağrısı üretir. Ada Is Akademi tarafında **ek** bir `Api.DependencyInjection.ServiceRegister : IServiceRegister` eklerseniz, aynı modüller iki kez kayıtlanabilir.
- **Kanıt:** Üretilen `GeneratedServiceRegistrar.AddAzoxiaCore` içinde zaten `new ...AdaIsAkademi.Application...ServiceRegister().Register(services)` ve `Persistence` eşleniği yer alıyor; ardından Core persistence/application/infrastructure kayıtları geliyor.
- **Etkisi:** Çift kayıt (özellikle `AddDbContext` / scoped handler) beklenmeyen çalışma zamanı davranışı.
- **Öneri:** Modül kayıtlarını yalnızca ilgili assembly’deki `IServiceRegister` ile genişletin; Api’de ikinci bir bileşik `ServiceRegister` tanımlamayın.

### [Medium] [Bakım] Manuel handler kaydı — sürüm kayması riski

- **Neden önemli:** Her yeni `*CommandHandler` / `*QueryHandler` için `ServiceRegister` içinde eşleşen `AddScoped` satırı gerekir; unutulursa `GetRequiredService<IRequestHandler<...>>` çalışma zamanında hata verir.
- **Kanıt:** `src/Application/DependencyInjection/ServiceRegister.cs` — `RegisterCommandHandlers` / `RegisterQueryHandlers` uzun manuel listeler.
- **Etkisi:** Geliştirici hatasıyla kolay regresyon.
- **Öneri:** PR şablonu maddesi, kaynak üretici veya küçük bir yansıma testi (“tüm `CommandHandlerBase<>` tipleri DI’da var mı?”).

### [Low] [Validation] `UpdateJobPostingCommand` validator (tamamlanan kısım)

- **Durum:** `WageAmount > 0` ve `ShiftEndTime > ShiftStartTime` kontrolleri validator’a eklendi (`AZX_ADA_APP_VAL_111`, `AZX_ADA_APP_VAL_112`).
- **Kalan:** İstenirse vardiya tarihi / para birimi ISO formatı gibi ek kurallar domain veya ek validator adımlarıyla genişletilebilir.

### [Medium] [Test] Application katmanı otomatik test kapsamı dar

- **Neden önemli:** Handler + validator birlikte davranış oluşturur; regresyon riski yüksek.
- **Kanıt:** `tests/ApplicationTests/` — `AdaIsCacheKeysTests`, `JobPostingApplicationValidatorsTests` (validator odaklı); komut/query handler mutlu yol + `NotFound` senaryoları yok.
- **Etkisi:** Refaktör ve DI değişiklikleri güvencesiz kalabilir.
- **Öneri:** Sahte `IUnitOfWork` veya test veritabanı ile Submit/Accept/Create vb. handler yolları; validation reddi senaryoları genişletme.

### [Low] [Okuma modeli] Sorgu sonuç tipleri kök `Application` namespace’inde

- **Neden önemli:** Geniş public yüzey; API ve Application aynı assembly paylaşınca ad çakışması ve evrim zorlaşır.
- **Kanıt:** Okuma modelleri `ModelBase` türevi `*Model` kayıtları olarak `Queries/.../*.cs` dosyalarında tutulur (örn. `EmployerDetailModel`, `GetEmployerByIdQuery.cs` ile aynı klasör).
- **Etkisi:** Büyüyen çözümde okunabilirlik ve sınır netliği azalır.
- **Öneri:** `.cursor/rules/application-layer.mdc` içindeki `*Model` dosya başına kuralına uyulması.

### [Low] [Gözlemlenebilirlik] `Logger` kullanılmıyor

- **Neden önemli:** `QueryHandlerBase` / `CommandHandlerBase` logger sağlıyor; çoğu handler yalın orkestrasyon.
- **Kanıt:** Örnek handler gövdeleri (`PublishJobPostingCommand`, `GetJobPostingByIdQuery`) log çağrısı içermiyor.
- **Etkisi:** Üretim izi sınırlı.
- **Öneri:** Yapılandırılmış log veya bilinçli “sessiz handler” politikası dokümante edilsin.

## CQRS Uygunluk Değerlendirmesi

- **Command/Query ayrımı:** Komutlar `CommandBase` + validator + `CommandHandlerBase`; sorgular `QueryBase<TResult>` + validator + `QueryHandlerBase` — **ayrım net ve tutarlı**.
- **Handler sorumluluk dağılımı:** Çoğu handler kısa orkestrasyon (repo + domain metodu + `SaveChangesAsync` / projeksiyon) — **SRP uyumu iyi**.
- **Transaction / UnitOfWork yönetimi:** Tek aggregate üzerinde tek `SaveChangesAsync` deseni korunuyor; çoklu aggregate işlemleri bu taramada görülmedi.
- **Validation ve hata stratejisi:** `ValidationPipelineBehavior` + `IRequestValidator<>` ile uyumlu; **UpdateJobPosting** temel alan doğrulamaları tamamlandı.

## Domain İşbirliği Değerlendirmesi

- **Domain kuralı sızıntıları:** İncelenen komut handler’ları iş kurallarını entity metodlarına delegasyon — **olumlu**.
- **Uygulama katmanında olması gereken vs olmaması gereken:** İşveren kapsamlı ilan başvuru uçlarında (`ListJobApplicationsByJobPostingIdQuery`, Accept/Reject) **işveren id eşlemesi** handler’da uygulanıyor (`EmployerId` vs `JobPosting.EmployerId`; uyumsuzluk `NotFound`). API şimdilik `employerId` query — **JWT/policy** gelene kadar `[AllowAnonymous]` ile zayıf güven; üretimde oturumdan doldurulmalı. Idempotency ayrıca ele alınmalı.
- **Okuma tarafı:** Sorgular `AsNoTracking()` kullanıyor; projeksiyon veya bellek içi DTO eşlemesi — **kabul edilebilir**; EF çevrilebilirliği için `ListOpenJobPostings` içindeki `ToListAsync(selector, …)` provider’a bağlı — izlenmeli.

## Aksiyon Planı

1. **Hızlı kazanımlar:** `GeneratedServiceRegistrar` ile çakışmayacak şekilde DI genişletme; Application testleri.
2. **Orta vadeli iyileştirmeler:** Application handler testleri; yeni handler için DI kontrolü (test veya generator).
3. **Kalıcı guardrail'ler:** PR checklist; isteğe bağlı read model namespace düzeni; loglama standardı.

---

*Kapsam: `src/Application` birincil; `src/Domain` ve Core Application/Persistence soyutlamaları destekleyici. Önceki rapora göre **kritik handler DI eksikliği** manuel kayıtlarla giderilmiş kabul edilir; bu rapor güncel kalan risklere odaklanır.*
