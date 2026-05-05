# Ada İş Akademi — kod yapısı gözden geçirme (plan / mimari envanter)

Bu dosya `/ada-is-akademi-plan` akışına uygun olarak **yalnızca analiz ve takip** içerir; kapsam dışı işler **onaysız uygulanmaz**.

## Goal

- Repodaki **katmanlı yapıyı** (Domain → Application → Persistence / Infrastructure → Api) `.cursor/rules` ile hizalı şekilde özetlemek.
- **Güçlü yönler**, **riskler** ve **isteğe bağlı iyileştirmeleri** ayırt etmek.
- Sonraki sprint/epik seçiminde kullanılmak üzere **tek sayfalık envanter** sağlamak.

## Out of scope (bu dokümanda)

- Derin güvenlik denetimi (OWASP taraması, penetration test).
- Performans profilleme ve üretim ölçümü.
- PRD’de deferred bırakılmış ürün epiklerinin uygulanması (ör. CV pipeline, AI profil analizi).
- `Azoxia.Core` monorepo içindeki geniş tasarım incelemesi (yalnızca bu repodaki referanslar).

## Yöntem

1. `.cursor/rules/*.mdc` (application, domain, api, persistence, infrastructure, static-members, tests) okundu.
2. `docs/tasks/ada-is-akademi-execution-tracker.md` güncel durum alındı.
3. `src/*` ve `tests/*` altında proje sınırları, DI kayıtları ve temsilî kalite kontrolleri (ör. API routing kalıbı) gözle tarandı.

## Çözüm topolojisi (özet)

| Proje / klasör | Yaklaşık `.cs` sayısı | Rol |
|----------------|----------------------|-----|
| `src/Domain` | ~57 | Aggregate’ler, enum’lar, value object’ler; yalnız `Core` referansı. |
| `src/Application` | ~200 | CQRS komut/sorgu, handler’lar, `ModelBase` read modelleri, `AdaIsCacheKeys`, validasyon kodları. |
| `src/Persistence` | ~65 | `AdaIsAkademiDbContext`, `Mapping/*Configuration`, EF migrations. |
| `src/Infrastructure` | ~8 | Harici sistem adaptörleri (ör. object storage presigner, push sahte gönderici). |
| `src/Api` | ~19 | Controller’lar, automation job tipleri, `Program` / host köprüsü. |
| `tests` | ~42 | Domain + Application testleri; handler test altyapısı (SQLite in-memory). |
| `tools/AdaIsAkademi.SeedRunner` | (ayrı csproj) | Veri tohumlama pipeline’ı. |

**Dış bağımlılık:** Uygulama ve persistence, üst dizindeki **`Core`** (`../../../Core/src/...`) üzerinden `Azoxia.Core.*` ile çalışır; API davranışının önemli kısmı Core’daki `Startup` / `ApiControllerBase` sözleşmesine dayanır.

## Katman bazında kural uyumu

### Domain

- **Uyum:** `Domain.csproj` yalnızca `Core` referansı taşır; Application/Persistence yok.
- **Uyum:** `Properties/AssemblyInfo.cs` ile `InternalsVisibleTo` Application ve testlere açık (factory/`protected internal` kullanımı için).
- **Not:** Kurallar `#region` sırası ve İngilizce XML özetleri ister; geniş entity dosyalarında (`Worker` vb.) sürdürülebilirlik için bölüm disiplinine devam edilmeli.

### Application

- **Uyum:** CQRS ayrımı, handler’ların `CommandHandlerBase` / `QueryHandlerBase` + `IServiceProvider` ile çalışması, `ServiceRegister` içinde handler/validator eşlemeleri yoğun ve tutarlı görünüyor.
- **Uyum:** Read/write şekilleri `*Model` + `ModelBase` hattında; cache anahtarları `AdaIsCacheKeys` altında toplanmış.
- **Uyum:** İş kuralları için DI servis örneği: `IWorkerEmployerProfileAccess` (static yardımcı yerine instance + arayüz).
- **Static üyeler:** `AdaIsCacheKeys`, `ApplicationValidationCodes`, extension/static yardımcı sınıflar kurallardaki **dar kapsamlı istisna** ile uyumlu sayılabilir. Beklenen maaş kolonlarından `Money` örneklemesi **`Worker`** üzerinde (`GetExpectedSalaryMinMoney` / `GetExpectedSalaryMaxMoney`) tutulur; Application’da ayrı static eşleme sınıfı kaldırıldı.

### Persistence

- **Uyum:** İş kuralı yok; mapping + `DbContext`, transaction sınırı `IUnitOfWork` ile.
- **Uyum:** Mapping sınıflarının `EntityTypeConfigurationBase<T>` kalıbı ve migration’ların repoda izlenmesi.
- **Risk (ürün/ops):** Şema büyüdükçe migration geçmişi ve snapshot birlikte taşınmalı; geri dönüş stratejisi ayrı dokümanda netleştirilmeli.

### Infrastructure

- **Uyum:** Uygulama sözleşmelerini uygular; domain şekillendirmez.
- **Uyum:** Konfigürasyon `IConfig` / `ObjectStorageConfig` gibi tiplerle okunur; sırlar kodda gömülü olmamalı (`.env` `.gitignore`’da).

### Api

- **Uyum:** Controller’larda ekstra `[Route("…")]` ve `[HttpPost("template")]` kalıbına rastlanmadı (örnek tarama); `ApiControllerBase` sözleşmesi korunuyor.
- **Uyum:** İş mantığı controller’da değil Application’da.
- **Not:** `Program.cs` içinde **DataProtection** dosya yolu (`/home/app/...`) ve **Hangfire InMemory** yapılandırması geliştirme/ Docker senaryosuna uygun; üretimde kalıcı storage ve güvenlik politikası tracker’daki “Deferred” maddelerle uyumlu ilerletilmeli.

### Tests

- **Uyum:** Application testleri gerçek EF SQLite + `UnitOfWork` ile handler davranışını doğruluyor.
- **Takip:** Yeni endpoint’ler için contract test (OpenAPI vs gerçek gövde) isteğe bağlı güçlendirme.

## Çapraz kesit — dikkat listesi (öncelik sırası yok)

1. **Core sürüm uyumu:** Core ile AdaIsAkademi aynı workspace’te; Core’da kırıcı değişiklik bu repoda dalga etkisi yaratır — yükseltmelerde birlikte doğrulama şart.
2. **Cache disiplini:** Application kuralı her yeni read/mutasyon için cache kararı ister; PR’larda özellikle list sorguları ve yeni aggregate’ler için gözden kaçmaması gerekir.
3. **Görev dokümantasyonu:** `docs/tasks/README.md` eklendi (konvansiyonlar ve tracker ilişkisi).
4. **Tracker senkronu:** Büyük teslimatlar sonrası `ada-is-akademi-execution-tracker.md` güncel tutulmalı (skill kuralı).

## Önerilen takip işleri (onay sonrası)

- [x] ~~İsteğe bağlı: beklenen maaş `Money` eşlemesi~~ — `Worker.GetExpectedSalaryMinMoney` / `GetExpectedSalaryMaxMoney` (Application `WorkerExpectedSalaryMappings` kaldırıldı).
- [x] ~~`docs/tasks/README.md`~~ — görev klasörü konvansiyonları eklendi.
- [ ] Ürün backlog: tracker’daki **CV pipeline** veya **deferred** maddelerden biri seçildiğinde ayrı `docs/tasks/<epik>.md` açılması.

## Done (bu oturum)

- [x] Katman kuralları okundu, çözüm ağacı sayıldı, API routing örnek taraması yapıldı.
- [x] Bu gözden geçirme dosyası `docs/tasks/codebase-structure-review-2026-05.md` olarak yazıldı.
- [x] `docs/tasks/README.md` eklendi; mimari gözden geçirme dosyasındaki takip maddeleri güncellendi.
- [x] Beklenen maaş read-model eşlemesi `Worker` entity’sine taşındı (`WorkerExpectedSalaryMappings.cs` kaldırıldı).

## Follow-ups

- Sonraki anlamlı kod değişikliğinde ilgili **epik görev dosyası** ve **execution tracker** satırları güncellensin.
