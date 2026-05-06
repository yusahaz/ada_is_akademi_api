# Ada İş Akademi - Frontend Endpoint Haritası

Bu doküman, frontend tarafında rol bazlı hangi endpointlerin çağrılması gerektiğini özetler.

Temel route kuralı:

- `/{Controller}/{Action}`
- Örnek: `SystemUsers/Login`, `Workers/GetSelfSummary`

---

## Public (anonim)

- `SystemUsers/Login`: E-posta/şifre ile giriş yapar, access + refresh token döndürür.
- `SystemUsers/Logout`: Refresh token’ı revoke ederek oturumu kapatır.
- `SystemUsers/RefreshToken`: Access token süresi dolunca yeni token çifti üretir.
- `SystemUsers/RegisterEmployer`: Employer hesabı + şirket kaydı açar.
- `SystemUsers/RegisterWorker`: Worker hesabı + worker profil kaydı açar.
- `SystemUsers/RequestEmailVerification`: Doğrulama token üretir/yeniler.
- `SystemUsers/VerifyEmail`: E-posta doğrulama token’ını onaylar.
- `JobPostings/GetById`: Tek bir ilanın detayını public olarak getirir.
- `JobPostings/ListOpen`: Public açık ilan listesini sayfalı getirir.

---

## Worker (JWT içinde `worker_id`)

### Profil / CV / Medya

- `Workers/GetSelfSummary`: Worker panel üst özetini (kısa profil) getirir.
- `Workers/GetSelfFullDetail`: Worker’ın tam profilini (detay alanlarla) getirir.
- `Workers/UpdateProfile`: Temel profil alanlarını (ad, soyad, uyruk, üniversite) günceller.
- `Workers/UpdateBio`: Worker “hakkımda” metnini günceller.
- `Workers/UpdateSocialLinks`: Worker sosyal link listesini komple replace eder.
- `Workers/UpdateMatchingPreferences`: Beklenen ücret + ilgi kategorilerini günceller.
- `Workers/InitProfilePhotoUpload`: Profil foto için presigned PUT URL üretir.
- `Workers/ConfirmProfilePhotoUpload`: Yükleme sonrası object key’i profile işler.
- `Workers/ClearProfilePhoto`: Profil foto metadata’sını temizler.
- `Workers/InitCvUpload`: CV dosyası için presigned PUT URL üretir.
- `Workers/ConfirmCvUpload`: CV upload metadata’sını kaydeder ve session açar.
- `Workers/ConfirmCvReview`: Çıkan CV extraction sonucunu onaylar.
- `Workers/DiscardCvReview`: Çıkan CV extraction sonucunu reddeder.

### Worker alt kayıtları

- `Workers/AddSkill` / `Workers/RemoveSkill`: Skill etiket ekler/siler.
- `Workers/AddAvailability` / `Workers/RemoveAvailability`: Müsaitlik slotu ekler/siler.
- `Workers/AddEducation` / `Workers/RemoveEducation`: Eğitim kaydı ekler/siler.
- `Workers/AddExperience` / `Workers/RemoveExperience`: Deneyim kaydı ekler/siler.
- `Workers/AddLanguage` / `Workers/RemoveLanguage`: Dil yetkinliği ekler/siler.
- `Workers/AddCertificate` / `Workers/RemoveCertificate`: Sertifika ekler/siler.
- `Workers/AddReference` / `Workers/RemoveReference`: Referans kaydı ekler/siler.

### İlan / Başvuru

- `JobPostings/ListOpen`: Açık ilanları filtre/paging ile listeler.
- `JobPostings/GetById`: İlan detayını getirir.
- `JobPostings/ListSemanticMatched`: Worker embedding’e göre uygun ilanları sıralar.
- `JobApplications/Submit`: İlana başvuru oluşturur.
- `JobApplications/Withdraw`: Pending başvuruyu geri çeker.
- `JobApplications/MyApplications`: Worker’ın kendi başvurularını listeler.

### Vardiya / Ödeme / Bildirim

- `ShiftAssignments/MyAssignments`: Worker’ın vardiya atamalarını listeler.
- `ShiftAssignments/CheckIn`: Worker QR check-in işlemini yapar.
- `ShiftAssignments/CheckOut`: Worker vardiya çıkışını yapar.
- `Workers/ConfirmPayout`: Worker payout transferini onaylar.
- `Workers/NotificationPreview`: Worker için bildirim önizlemesi üretir.

---

## Employer (JWT içinde `employer_id`)

### Worker görüntüleme (employer-safe)

- `Workers/GetById`: Employer-safe worker kısa kart datası getirir.
- `Workers/GetDetail`: Employer-safe worker detay profilini getirir.
- `Workers/RecordEmployerWorkerProfileView`: Worker profil görüntüleme istatistiği yazar.
- `Workers/GetProfilePhotoViewUrl`: Worker fotoğrafı için kısa ömürlü GET URL döner.

### İş ilanı yönetimi

- `JobPostings/Create`: Draft ilan oluşturur.
- `JobPostings/Update`: Draft ilan alanlarını günceller.
- `JobPostings/Publish`: Draft ilanı yayına alır.
- `JobPostings/Cancel`: İlanı iptal eder.
- `JobPostings/Complete`: İlanı tamamlandı durumuna alır.
- `JobPostings/AddSkill`: İlana required skill ekler.
- `JobPostings/RemoveSkill`: İlandan skill siler.
- `JobPostings/ListByEmployer`: Auth employer’a ait ilanları listeler.

### Başvuru yönetimi

- `JobApplications/List`: İlana gelen başvuruları listeler.
- `JobApplications/Accept`: Başvuruyu kabul eder.
- `JobApplications/Reject`: Başvuruyu reddeder.

### Vardiya / Ödeme / Komisyon

- `ShiftAssignments/Create`: Kabul edilmiş başvurudan vardiya ataması üretir.
- `ShiftAssignments/SupervisorCheckIn`: Supervisor QR check-in onayı verir.
- `Employers/CreateWorkerPayout`: Assignment üzerinden payout kaydı açar.
- `Employers/MarkWorkerPayoutAsProcessing`: Payout’u processing’e çeker.
- `Employers/FailWorkerPayout`: Payout’u failed yapar (neden ile).
- `Employers/RetryWorkerPayout`: Failed payout için retry başlatır.
- `Employers/GenerateCommissionReceivable`: Dönemsel komisyon alacağı üretir.
- `Employers/GetCommissionEstimate`: Komisyon tahmin özetini getirir.
- `Employers/GetCommissionPolicy`: Employer komisyon oran politikasını getirir.
- `Employers/GetCommissionReceivableByPeriod`: Belirli dönem alacak detayını getirir.
- `Employers/ListCommissionReceivables`: Employer alacak satırlarını listeler.
- `Employers/ListCommissionSummaries`: Komisyon listesi/özet satırlarını getirir.
- `Employers/ExportCommissionPoliciesCsv`: Komisyon politikalarını CSV paket döner.

### Employer profil / organizasyon

- `Employers/GetById`: Employer kısa detay döner.
- `Employers/GetDetail`: Employer tam profil, lokasyon ve supervisor detaylarını döner.
- `Employers/AddLocation`: Yeni şube/lokasyon ekler.
- `Employers/AddSupervisor`: Supervisor kullanıcıyı employer’a bağlar.
- `Employers/RemoveSupervisor`: Supervisor bağını kaldırır.
- `Employers/UpdateSocialLinks`: Şirket sosyal linklerini replace eder.
- `Employers/InitLogoUpload`: Logo için presigned PUT URL üretir.
- `Employers/ConfirmLogoUpload`: Logo object key’i employer profiline işler.
- `Employers/ClearLogo`: Logo metadata’sını temizler.
- `Employers/GetLogoViewUrl`: Logo için kısa ömürlü GET URL döner.

---

## Admin

### Kullanıcı yönetimi

- `SystemUsers/List`: Kullanıcıları filtre/paging ile listeler.
- `SystemUsers/Me`: Giriş yapan kullanıcı profilini döner.
- `SystemUsers/Ban`: Hesabı ban durumuna alır.
- `SystemUsers/Suspend`: Hesabı suspend durumuna alır.
- `SystemUsers/Reactivate`: Suspend/ban dışı uygun hesabı tekrar aktifleştirir.
- `SystemUsers/ChangePassword`: Kullanıcı şifresini değiştirir.
- `SystemUsers/RegisterAdmin`: Yeni admin hesap oluşturur.
- `SystemUsers/SendNotification`: Sistem kullanıcısına bildirim gönderir.
- `SystemUsers/MyNotifications`: Giriş yapan kullanıcının bildirim inbox listesini (paging + `isRead` filtresi) döner.
- `SystemUsers/MarkNotificationAsRead`: Bildirimi okundu olarak işaretler.
- `SystemUsers/MarkAllNotificationsAsRead`: Kullanıcının tüm okunmamış bildirimlerini okundu yapar.

### Grup / Yetki

- `SystemUserGroups/List`: Grup listesini filtre/paging ile döner.
- `SystemUserGroups/Activate`: Grubu aktif eder.
- `SystemUserGroups/Deactivate`: Grubu pasif eder.
- `SystemUserGroups/AddPermission`: Gruba izin etkisi (allow/deny) ekler.

### Yönetim listeleri / istatistik

- `Workers/List`: Worker listesi (admin görünümü).
- `Workers/Delete`: Worker soft delete (bağlı system user ile).
- `Workers/SendNotification`: Worker’a admin tarafından bildirim gönderir.
- `Employers/List`: Employer listesi (admin görünümü).
- `Employers/Activate`: Employer’ı aktif eder.
- `Employers/Ban`: Employer’ı banlar.
- `Employers/Suspend`: Employer’ı suspend eder.
- `Employers/Delete`: Employer soft delete.
- `Statistics/Overview`: Dashboard üst sayaçları.
- `Statistics/OverdueSummary`: Gecikmiş iş/başvuru özet sayaçları.
- `Statistics/MonetizationSummary`: Monetization ana metrik özeti.
- `Statistics/FinancialReconciliationSummary`: Finansal mutabakat özet metrikleri.
- `Statistics/FinancialReconciliationRows`: Mutabakat satır listesi (paging).
- `Statistics/ExportOverdueAlarmsCsv`: Overdue alarm CSV paketi.
- `Statistics/ExportSystemUserNotificationDispatchesCsv`: Notification dispatch CSV paketi.

---

## Frontend kritik not

- Worker oturumunda `Workers/GetDetail` çağrılmamalı.
- Worker için doğru endpointler: `Workers/GetSelfSummary` veya `Workers/GetSelfFullDetail`.
- Employer oturumunda worker detay için `Workers/GetDetail` kullanılmalı.

