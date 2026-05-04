# Application CQRS Doctor Report

> **Not:** Bu dosya **baseline** anlık görüntüsüdür; güncel mimari, DI ve test durumu için `application_cqrs_doctor_result_post-queries.md` esas alınmalıdır.

## Yönetici Özeti

- **Genel risk seviyesi:** Critical
- **Sistemik CQRS/Application sorunları:** Komut işleyicilerinin (`IRequestHandler<,>`) DI kapsayıcısına kayıtlı olmadığına dair güçlü kod kanıtı; okuma tarafında (`Query`) uç veya handler yokluğu; üretilen `AddAzoxiaCore` deseninin Ada Is Akademi modül `IServiceRegister` tiplerini host assembly dışında bırakma riski.
- **Öncelikli aksiyonlar:** Tüm komut handler’larını `IRequestHandler<TCommand, Unit>` (veya Core’daki eşdeğer arayüz) ile `AddScoped` olarak kaydetmek; host’ta Application + Persistence (+ Core Application) kayıtlarının tek giriş noktasından çağrıldığını doğrulamak; okuma senaryoları için `Query` + `QueryHandlerBase` eklemek.

## Bulgular

### [Critical] [DI / Çalışma Zamanı] Komut handler’ları DI’da kayıtlı değil

- **Neden önemli:** `Azoxia.Core.Application.RequestHandlerWrapper`, işleyiciyi `serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>()` ile çözüyor. Bu kayıt yoksa `SendAsync` çağrısı çalışma zamanında patlar; CQRS hattı fiilen kullanılamaz.
- **Kanıt:** `c:\WorkingFolder\Azoxia\Core\src\Application\RequestHandlerWrapper.cs` (satır 24–26); Ada Is Akademi ağacında `IRequestHandler` için `AddScoped` / `AddTransient` eşlemesi yok (`src/Application/DependencyInjection/ServiceRegister.cs` yalnızca `IRequestValidator<>` kaydediyor; `src/Api` ve `src/Persistence` içinde handler kaydı yok).
- **Etkisi:** API’den `ISender` ile gönderilen tüm komutlar üretimde kırılır (veya hiç denenmemiş kalır).
- **Öneri:** Her `*CommandHandler` için `services.AddScoped<IRequestHandler<TCommand, Unit>, THandler>()` (Core’daki gerçek imza ne ise ona uygun) kaydı ekleyin; mümkünse yansıma veya kaynak üretici ile tekilleştirin, yoksa `ServiceRegister.Register` içinde açık liste tutun.

### [High] [CQRS Tamamlık] Query / okuma tarafı tanımlı değil

- **Neden önemli:** CQRS’te komutlar durumu değiştirir; raporlama, listeleme ve detay okumaları genelde `Query` ile ayrılır. Yalnızca komut katmanı, okuma modeli ve API sözleşmesi eksik kalır.
- **Kanıt:** `src/Application` altında `QueryBase` / `IQuery` kullanan dosya yok (workspace taraması).
- **Etkisi:** Controller’lar doğrudan repository veya DbContext’e kayabilir (katman ihlali) veya özellikler ertelenir.
- **Öneri:** Okuma use-case’leri için `QueryBase` + `QueryHandlerBase<TQuery,TResult>` ve DTO/Projection; API’de yalnızca `ISender` üzerinden okuma.

### [High] [DI / Modül Kayıt] Host assembly dışındaki `IServiceRegister` örnekleri `AddAzoxiaCore` ile otomatik toplanmayabilir

- **Neden önemli:** `DependencyInjectionGenerator`, **derlenen assembly** içindeki `IServiceRegister` uygulayan sınıfları üretilen `AddAzoxiaCore` metoduna yazar. `Azoxia.AdaIsAkademi.Application.DependencyInjection.ServiceRegister` ve `Persistence` içindeki kayıt sınıfları **Api projesinin derlemesi dışında** kaldığı için, yalnızca `Program` içindeki `AddAzoxiaCore` çağrısıyla otomatik çalışmayabilir.
- **Kanıt:** `c:\WorkingFolder\Azoxia\Core\src\Generators\DependencyInjectionGenerator.cs` (derleme başına `IServiceRegister` tarama); Ada Is Akademi `src/Api` kaynaklarında `IServiceRegister` implementasyonu görünmüyor; `Application/DependencyInjection/ServiceRegister.cs` ayrı assembly’de.
- **Etkisi:** `ISender`, `ValidationPipelineBehavior`, `IUnitOfWork`, doğrulayıcılar veya handler kayıtları eksik kalabilir (ortama göre değişir).
- **Öneri:** Api’de tek bir “bileşik” `IServiceRegister` ile `new Application.ServiceRegister().Register(services)` ve `new Persistence.ServiceRegister().Register(services)` çağrılarını garanti edin; veya generator’ı çoklu assembly’yi kapsayacak şekilde genişletin (Core tarafında tasarım kararı).

### [Medium] [Validation] `UpdateJobPostingCommand` doğrulaması alan bütünlüğünü tam kapsamıyor

- **Neden önemli:** Domain `JobPosting.Update` yalnızca taslak durumunu doğrular; vardiya saat sırası, pozitif ücret gibi iş kuralları komut doğrulamasında veya domain’de net değilse hatalı veri kalabilir.
- **Kanıt:** `src/Application/Commands/JobPosting/UpdateJobPostingCommand.cs` — validator `JobPostingId`, başlık, açıklama, `HeadCount`, `WageCurrency` kontrol ediyor; `WageAmount > 0`, `ShiftEndTime > ShiftStartTime` gibi kontroller yok; `JobPosting.Update` (`src/Domain/Entities/JobPosting.cs`) bu alanlar için ek invariant uygulamıyor.
- **Etkisi:** Geçersiz ücret veya saat aralığı ile güncelleme persistence’a kadar gidebilir (Money tipi içinde ek kontrol varsa kısmen sınırlanır — ayrı doğrulanmalı).
- **Öneri:** Validator’da shift zaman penceresi ve ücret tutarı; gerekirse domain’de `Money` / süre için ortak invariant.

### [Medium] [Hata modeli] Uygulama ve domain hata kodları iki aile halinde

- **Neden önemli:** Handler’lar genelde `AzoxiaErrorCodes.NotFound` kullanıyor; domain davranışı `DomainErrorCodes` fırlatıyor. API/Problem Details eşlemesi ve gözlemlenebilirlik için tutarlı bir hiyerarşi gerekir.
- **Kanıt:** Örn. `PublishJobPostingCommandHandler` — `AzoxiaErrorCodes.NotFound`; `JobPosting.Publish` — `DomainErrorCodes.JobPostingInvalidStatusTransition`.
- **Etkisi:** İstemci ve log tarafında kod karmaşası; aynı senaryo için farklı HTTP/Problem çıktıları riski.
- **Öneri:** Domain hatalarının uygulama sınırında nasıl sarılacağını (mapping tablosu) dokümante edin; mümkünse tek katalog veya önek standardı (`AZX_ADA_APP_*` vs `AZX_ADA_DOMAIN_*`) koruyun.

### [Low] [Test / SRP] Application katmanı için otomatik test yok

- **Neden önemli:** Handler orkestrasyonu ve doğrulama birlikte davranış oluşturur; regresyon riski yüksek.
- **Kanıt:** `tests/` altında yalnızca `DomainTests` (ör. `JobPostingDomainTests.cs`); `ApplicationTests` projesi varsa handler testi üretilmiyor (bu baseline’da uygulama testi dosyası yok).
- **Etkisi:** Refaktör ve DI değişiklikleri kırılmadan geçebilir.
- **Öneri:** Sahte `IUnitOfWork` / in-memory veya test container ile en az “mutlu yol + NotFound + domain red” senaryoları.

### [Low] [Gözlemlenebilirlik] `Logger` enjekte ediliyor ancak handler gövdelerinde kullanılmıyor

- **Neden önemli:** İşlem izi ve hata teşhisi zorlaşır.
- **Kanıt:** `CommandHandlerBase` `Logger` sağlıyor; örnek handler’lar (`AddWorkerSkillCommand`, `PublishJobPostingCommand`) yalnızca yükle-değiştir-kaydet.
- **Etkisi:** Üretimde davranış analizi sınırlı.
- **Öneri:** Yapılandırılmış log (command adı, entity id) veya açıkça “bilerek sessiz” politikası.

## CQRS Uygunluk Değerlendirmesi

- **Command/Query ayrımı:** Komutlar `CommandBase` + dosya içi `*Validator` + `CommandHandlerBase` ile iyi ayrılmış; **Query tarafı eksik** — CQRS tam değil.
- **Handler sorumluluk dağılımı:** Handler’lar kısa orkestrasyon (repository’den yükle, domain metodu, `SaveChangesAsync`) — **SRP açısından uygun**; iş kuralları çoğunlukla domain entity’lerde (`JobPosting`, `Worker`, `SystemUser` vb.).
- **Transaction / UnitOfWork yönetimi:** Tek aggregate üzerinde tek `SaveChangesAsync` — tipik ve tutarlı; çok aggregate işlemleri için açık transaction politikası bu raporda doğrulanmadı (kullanım yok).
- **Validation ve hata stratejisi:** `ValidationPipelineBehavior` tüm `IRequestValidator<>` örneklerini çalıştırıyor — **tasarım doğru**; validator kayıtları `Application.ServiceRegister` içinde; **handler kayıtları eksik** olduğu sürece pipeline’a gelen istekler yine de kırılır.

## Domain İşbirliği Değerlendirmesi

- **Domain kuralı sızıntıları:** İncelenen örneklerde (`PublishJobPostingCommandHandler`, `AcceptJobPostingApplicationCommandHandler`, `AddWorkerSkillCommandHandler`) iş kuralı domain metodlarına delegasyon — **olumlu**.
- **Uygulama katmanında olması gereken vs olmaması gereken:** Kimlik doğrulama / yetkilendirme / idempotency anahtarı gibi use-case politikaları handler’da veya ayrı davranışlarda olmalı — **mevcut kodda görünmüyor** (ileri güvenlik riski; bu raporda “kanıt yok” ile sınırlı).
- **Çift doğrulama:** Örn. `AddWorkerSkillCommand` tag boşluğu hem validator’da hem `SkillTag` value object içinde — **kabul edilebilir savunma derinliği**; fazlalık değil, katmanlar arası netlik için mesajların hizalanması yeterli.

## Aksiyon Planı

1. **Hızlı kazanımlar:** Tüm `*CommandHandler` sınıflarını DI’a kaydet; `ISender.SendAsync` ile uçtan uca duman testi; host’ta Application + Persistence (+ gerekirse Core Application) `Register` zincirini doğrula.
2. **Orta vadeli iyileştirmeler:** Okuma query’leri ve handler’ları; `UpdateJobPostingCommand` validator genişletmesi; hata kodu eşleme dokümantasyonu.
3. **Kalıcı guardrail'ler:** Application handler birim testleri; CI’de “handler kaydı için yansıma testi” veya kaynak üretici; CQRS checklist PR şablonu.

---

*Rapor kapsamı: `src/Application` birincil; `src/Domain` ve `Core/src/Application` destekleyici. Çalışma zamanı davranışı için DI kayıtları codebase statik analizi ile teyit edilmiştir; üretim ortamında ek bootstrap dosyaları varsa ayrıca doğrulanmalıdır.*
