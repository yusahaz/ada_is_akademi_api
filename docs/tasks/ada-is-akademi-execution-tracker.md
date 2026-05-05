# Ada İş Akademi - Execution Tracker

Bu dosya, her iş tamamlandığında güncellenen **tek kaynak** takip alanıdır.
`/ada-is-akademi-plan` çalıştırıldığında önce bu dosya okunur ve plan kaldığı yerden devam ettirilir.

## Kullanım Kuralı
- Her tamamlanan işten sonra yalnızca ilgili satırları güncelle.
- Yeni kapsam eklenirse `Deferred Backlog` bölümüne ekle.
- Faz geçişlerinde `Current Phase` ve `Current Task` alanlarını birlikte güncelle.
- Her güncellemede `Last Updated` alanına tarih/saat yaz.

## Current Status
- Last Updated: 2026-05-05 17:56 (UTC+3)
- Current Phase: Faz 1 eksiklerinin kapanışı
- Current Task: Sprint 3 mutual QR check-in/out handshake tamamlandı (worker+supervisor çift onay)
- Next Task: WorkerPayout lifecycle ve finansal race/idempotency sertleştirme
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

## Deferred Backlog
- [ ] Finansal mutabakat ve ileri raporlama detayları (ayrı faza alınacak)
- [ ] Query splitting/performance warning cleanup (Sprint 1/2 sonrası teknik borç)
- [ ] DataProtection key encryption policy (production hardening)

## Session Handoff Template
Her yeni oturum başında bu blok güncellenir:

- Handoff Summary:
- Nerede kaldık:
- Bir sonraki tek adım:
- Risk/Not:
