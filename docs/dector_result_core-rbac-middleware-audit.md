# Doctor Report

## Executive Summary
- Overall risk: **High**
- Key systemic concerns: Yetki kontrolü middleware seviyesinde uygulanmış olsa da anonim istekleri pas geçmesi, gelecekte `RequiresPermission` kullanılan endpoint'lerde yanlış konfigürasyon durumunda yetki bypass riskini artırıyor. Ayrıca reflection-temelli çözüm katman bağımlılığını kırılgan hale getiriyor.
- Immediate actions: `RequiresPermission` bulunan isteklerde anonim erişimi açıkça engelleyin (401/403), permission resolver çağrısını güçlü tipli hale getirin ve `Api.csproj` içindeki mükerrer derleme girdilerini temizleyin.

## Findings
### [High] [Security/AuthZ] Anonim istekler permission kontrolünü atlayabiliyor
- **Why it matters:** Permission attribute'ü endpoint üzerinde olsa bile kullanıcı doğrulanmamışsa middleware kontrol yapmadan isteği bir sonraki adıma geçiriyor. Bu durum, endpoint tarafında `Authorize` unutulursa doğrudan yetki bypass'a dönüşebilir.
- **Evidence:** `Core/src/Api/Middleware/PermissionEnforcementMiddleware.cs` içinde `IsAuthenticated == false` durumunda `await _next(context)` ile erken çıkış.
- **Impact:** Yanlış endpoint konfigürasyonunda yetkisiz kullanıcıların korumalı işlevlere erişmesi (privilege escalation / broken access control).
- **Recommendation:** `RequiresPermission` mevcutsa anonim çağrıyı doğrudan `401` (veya politika gereği `403`) ile sonlandırın; ek olarak startup aşamasında `RequiresPermission` kullanılan action'lar için `Authorize` zorunluluğunu doğrulayan bir guard ekleyin.
- **Optional references:** CWE-285, OWASP A01:2021 (Broken Access Control)

### [Medium] [Architecture/Reliability] Reflection ile resolver çağrısı kırılgan ve hata yönetimi zayıf
- **Why it matters:** Resolver tipi assembly adıyla string üzerinden bulunuyor ve method reflection ile invoke ediliyor. Bu yaklaşım refactor/rename senaryolarında sessiz kırılma ve operasyonel belirsizlik üretir.
- **Evidence:** `Core/src/Api/Middleware/PermissionEnforcementMiddleware.cs` içinde `Type.GetType(...)`, `GetMethod("HasPermissionAsync")`, `MethodInfo.Invoke(...)` zinciri.
- **Impact:** Runtime'da beklenmeyen exception'lar (`TargetInvocationException` vb.) 500'e düşebilir; üretimde tanılama zorlaşır, erişim kontrolü davranışı öngörülemez hale gelir.
- **Recommendation:** Permission kontrolü için Core katmanında minimal bir kontrat tanımlayıp DI ile güçlü tipli servis enjekte edin; reflection çağrılarını kaldırın. Hata durumlarını açık şekilde `AzoxiaException(PermissionDenied)` veya `Unauthorized` semantiğine map edin.
- **Optional references:** CWE-670 (Always-Incorrect Control Flow Implementation)

### [Low] [Build/Code Quality] Mükerrer Compile girdileri derleme uyarısı üretiyor
- **Why it matters:** SDK-style projelerde dosyalar varsayılan olarak derlemeye dahil edilir; tekrar `Compile Include` yazılması gürültülü CI çıktısı ve teknik borç oluşturur.
- **Evidence:** `Core/src/Api/Api.csproj` içindeki `Compile Include="Authorization/RequiresPermissionAttribute.cs"` ve `Compile Include="Middleware/PermissionEnforcementMiddleware.cs"`; build çıktısında `CS2002` uyarıları.
- **Impact:** CI sinyal kalitesi düşer, gerçek uyarı/hata tespiti zorlaşır.
- **Recommendation:** İlgili `Compile Include` satırlarını kaldırın (özel bir dahil etme politikası yoksa).
- **Optional references:** N/A

## Architecture & Design Assessment
- Strengths
  - Yetki hatalarının merkezi exception pipeline ile tek formata (`ApiResponse`) çevrilmesi tutarlılığı artırıyor.
  - Permission kontrolünün middleware katmanında merkezileştirilmesi, tekrar eden controller kodunu azaltma yönünde doğru bir niyet gösteriyor.
- Weaknesses
  - Core API katmanının application katmanına string/reflection üzerinden dolaylı bağlanması sınırları netleştirmiyor.
  - Güvenlik açısından kritik davranış (`RequiresPermission` + anonim istek) fail-open karakterde.
- Pattern and boundary observations
  - CQRS/katmanlı yapı korunuyor; ancak permission resolver kontratının resmi bir abstraction olarak Core tarafında tanımlanması boundary ihlalini azaltır.

## SOLID & DRY Assessment
- **Single Responsibility (S):** `PermissionEnforcementMiddleware` hem claim parsing, hem service discovery, hem invocation orchestration yapıyor; sorumluluklar ayrıştırılabilir.
- **Dependency Inversion (D):** Yüksek seviye politika (yetki kontrolü) somut assembly adına ve reflection detayına bağlı; interface tabanlı DI ile iyileştirilmeli.
- **DRY:** Şu an belirgin kopya iş kuralı görülmüyor; ancak string bazlı resolver erişimi benzer yerlerde tekrar ederse bakım maliyeti hızla artar.

## Security Posture
- Top risk, erişim kontrolünde yanlış konfigürasyona açık fail-open davranış.
- İkinci risk, reflection çağrısının güvenlik-kritik akışta deterministik olmayan hata modları üretmesi.
- Öncelik sırası: (1) anonim bypass'ı kapat, (2) güçlü tipli resolver entegrasyonu, (3) build hijyenini temizleyip CI sinyalini güçlendir.

## Action Plan
1. Quick wins (today/this sprint)
   - `RequiresPermission` için anonim erişimi engelleyen net davranış ekle (`401/403`).
   - `Api.csproj` mükerrer `Compile Include` girdilerini kaldır.
2. Structural improvements (medium term)
   - Permission resolver için Core katmanında abstraction tanımla; middleware'i reflection'dan çıkarıp DI ile güçlü tipli çağrıya taşı.
   - Claim doğrulama ve permission kararını ayrı servislere bölerek middleware karmaşıklığını azalt.
3. Guardrails (tests, linters, policies, CI checks)
   - Entegrasyon testleri: `RequiresPermission` + anonim istek, geçerli/geçersiz claim, permission deny/allow senaryoları.
   - Mimari kural testi: `RequiresPermission` kullanılan endpoint'lerin `Authorize` politikası taşıdığını doğrula.
   - CI’de warning budget veya seçili uyarıları hata seviyesine yükseltme (özellikle derleme girdisi duplikasyonları).
