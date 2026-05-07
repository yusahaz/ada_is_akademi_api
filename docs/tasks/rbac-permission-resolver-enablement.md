# RBAC Permission Resolver Enabling Plan

## Goal
Sisteme giriş yapmış kullanıcıların, `SystemUserGroup` + `Permission` (izin/etki: Allow/Deny, scope-aware üyelik) modeline göre **ince yetkilendirme** ile API operasyonlarını kullanabilmesini sağlamak.

PRD’nin hedeflediği şekilde `PermissionResolver` mantığı çalışacak ve gerektiğinde Redis tabanlı cache invalidation yapılacak.

## Current Status (repo gözlemleri)
- `JWT Bearer` kimlik doğrulama çalışıyor:
  - `Core/src/Api/Startup.cs`: `AddAzoxiaJwtBearerAuthentication`, `UseAuthentication`, `UseAuthorization`, `AddAuthorization`.
  - `Core/src/Api/Controllers/ApiControllerBase.cs`: tüm controller’larda varsayılan `[Authorize]`.
- “Actor id” bazlı temel yetkilendirme/erişim kısıtı var:
  - `worker_id` / `employer_id` / `system_user_id` claim’leri `IExecutionContext` üzerinden okunuyor ve eksikse `ApplicationValidationCodes.*ClaimRequired` ile hata dönülüyor.
- Ancak “group-based permission evaluation” için PRD’de bahsi geçen `PermissionResolver` ve endpoint bazlı permission enforcement şu anda kodda entegre görünmüyor:
  - `SystemUserGroupsController` yalnızca authentication ile korunuyor; grup/permission ile diğer endpoint’lerde erişim kısıtı uygulanmıyor.

## Out of Scope (bu planın dışında)
- Mevcut endpoint sözleşmelerini (CQRS request/response body formatı) değiştirmek.
- Frontend tarafında yeni contract tasarlamak (sadece hangi endpoint’in hangi permission’a ihtiyaç duyduğunu dokümante etmek).
- Redis olmadan çalışan “tam çözüm” (Redis dependency opsiyonel olabilir ama cache entegrasyon tasarımı hedeflenir).

## Plan / Task List
1. **Permission gereksinimi sözleşmesini tanımla**
   - PRD’de tarif edilen `resource.action` formatına uygun permission string’leri belirle.
   - Her API endpoint/aksiyon için gerekli permission’ı belirleyecek bir mekanizma seç:
     - Öneri: controller/action’a attribute (ör. `RequiresPermission("workers.list")`) veya action metadata.
   - (Opsiyonel) CQRS handler/validator seviyesinde de aynı attribute ile ilerlenip ilerlenmeyeceğini netleştir.

2. **Permission resolution mantığını uygulayacak servis tasarla**
   - Yeni servis: `IPermissionResolver` (ve/veya `PermissionResolver`)
   - Girdi: `system_user_id` (claim), `scope` (global/employer/worker ile ilişkilendirilecek).
   - Çıktı: “given permission is allowed?” karar modeli.
   - Değerlendirme kuralları:
     - `PermissionEffect`: Allow/Deny
     - PRD’ye göre hierarchy/level/deny-override kuralını netleştir (Deny her zaman kazanır yaklaşımını implement et).
     - `SystemUserGroup.IsActive` + `SystemUserGroupMembership.IsActive` filtresi uygula.

3. **Cache (Redis L2) entegrasyonu**
   - `Core/src/Infrastructure/Services/CacheService.cs` iki katmanlı L1/L2 cache sağlıyor; bunu kullan.
   - Cache key tasarımı:
     - minimum: `system_user_id` + (gerekiyorsa) scope discriminator.
   - Permission cache invalidation:
     - grup permission değişince ilgili cache key’leri temizle.
     - membership değişince cache invalidation tasarımını da kapsa.

4. **Enforcement: endpoint’te yetki kontrolünü bağla**
   - `Core`/`Api` tarafında middleware veya ASP.NET Authorization handler yaklaşımı:
     - Endpoint metadata’dan gerekli permission’ı oku.
     - Handler içinde `IPermissionResolver` çağır ve sonucu karar ver.
   - Başarısız durumda dönüş biçimini PRD/envelope standartlarıyla uyumlu yap (global exception handler üzerinden `ApiResponse` şekli).

5. **Endpoint inventory + permission mapping**
   - `src/Api/Controllers/*` içinde admin/operasyonel endpoint’leri sırayla tarayıp gerekli permission’ı ekle.
   - Özellikle:
     - `SystemUserGroupsController` (RBAC yönetimi)
     - admin list/delete/suspend/notification/statistics endpointleri
   - Worker/Employer “self” endpointleri için permission gerekip gerekmeyeceğini ürün tarafı ile netleştir.

6. **Test planı**
   - Unit test:
     - Permission resolver karar semantiği:
       - Deny override
       - Level stacking (varsa)
       - Hierarchy inheritance (varsa)
       - Membership scope (global/employer/worker) etkisi
   - Integration test (API):
     - JWT ile authenticated ol, permission yoksa endpoint 403/authorization failure dönsün.
     - Cache hit/miss ve invalidation regresyonları için en az 1 senaryo.

## Acceptance Criteria
- Yetkilendirme için sadece “authenticated” kontrolü değil, PRD’deki `SystemUserGroup` + `Permission` matrisine göre gerçek permission enforcement çalışır.
- En az bir admin endpoint:
  - permission’ı olan kullanıcı için başarılı,
  - permission’ı olmayan kullanıcı için başarısız olur.
- Cache invalidation mekanizması en az “group permission değişince” senaryosunda doğru çalışır.

## Open Questions
- Permission string formatı kesin mi? (Örn. `system.users.list`, `system.users.manage.groups` gibi)
- Hierarchical permission çözümleme kuralı PRD’de tam olarak “node + parent traversal” mı, yoksa sadece aynı node üzerinden mi?
- Scope mapping:
  - employer_id claim’i admin mi yoksa employer aktörleri için mi kullanılacak?
  - worker_id claim’i hangi endpoint’lerde hangi permission scope’una map edilecek?

