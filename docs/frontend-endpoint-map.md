# Ada Is Akademi - Frontend Endpoint Sozlesmesi

Bu dosya frontend ekipleri icin tek referans endpoint sozlesmesidir.

Model alan detaylari (property-level) icin:

- `docs/frontend-endpoint-model-catalog.md`

## Genel kural

- Route: `/{Controller}/{Action}`
- Govde tipi: tum endpointler JSON body alir (`POST`, sadece `JobPostings/Update` = `PUT`)
- Auth: aksi yazmiyorsa `Bearer` zorunlu
- Basari envelope:
  - Tekil: `ApiResponse<T>`
  - Komut/no-content: `ApiResponse`
  - Sayfali: `PageableApiResponse<TItem>`
- Hata envelope: `ApiResponse` (`400/404`)

## Frontend istek formati

Her endpointte asagidaki sirayla implement edin:

1. `Authorization`:
   - Public endpointlerde header yok.
   - Digerlerinde `Authorization: Bearer <accessToken>`.
2. `Content-Type: application/json`
3. Body:
   - `Request Model` sinifina uygun JSON.
4. Response parse:
   - `success/message/data` envelope parse edilir.
5. Zorunlu alan kontrolu:
   - `src/Application/**/<RequestModel>Validator.cs` dosyasindaki `NotEmpty/NotNull` kurallari frontendde de zorunlu olmalidir.

## Public endpointler

- `POST SystemUsers/Login`
  - Request Model: `LoginSystemUserCommand`
  - Zorunlu alanlar: login credential alanlari (validator bazli)
  - Donus: `ApiResponse<SystemUserTokenModel>`
- `POST SystemUsers/Logout`
  - Request Model: `LogoutSystemUserCommand`
  - Donus: `ApiResponse`
- `POST SystemUsers/RefreshToken`
  - Request Model: `RefreshSystemUserTokenCommand`
  - Donus: `ApiResponse<SystemUserTokenModel>`
- `POST SystemUsers/RegisterEmployer`
  - Request Model: `RegisterEmployerCommand`
  - Donus: `ApiResponse<int>` (olusan employer/user id)
- `POST SystemUsers/RegisterWorker`
  - Request Model: `RegisterWorkerCommand`
  - Donus: `ApiResponse<int>` (olusan worker/user id)
- `POST SystemUsers/RequestEmailVerification`
  - Request Model: `RequestSystemUserEmailVerificationCommand`
  - Donus: `ApiResponse`
- `POST SystemUsers/VerifyEmail`
  - Request Model: `VerifySystemUserEmailCommand`
  - Donus: `ApiResponse`
- `POST JobPostings/GetById`
  - Request Model: `GetJobPostingByIdQuery`
  - Donus: `ApiResponse<JobPostingDetailModel>`
- `POST JobPostings/ListOpen`
  - Request Model: `ListOpenJobPostingsQuery` (bos body `{}` destekli)
  - Donus: `PageableApiResponse<JobPostingSummaryModel>`

## Worker endpointleri (`worker_id` claim)

### Profil / medya / CV

- `POST Workers/GetSelfSummary` -> `GetWorkerSelfDetailQuery` -> `ApiResponse<WorkerSelfDetailModel>`
- `POST Workers/GetSelfFullDetail` -> `GetWorkerSelfFullDetailQuery` -> `ApiResponse<WorkerSelfFullDetailModel>`
- `POST Workers/UpdateProfile` -> `UpdateWorkerProfileCommand` -> `ApiResponse`
- `POST Workers/UpdateBio` -> `UpdateWorkerBioCommand` -> `ApiResponse`
- `POST Workers/UpdateSocialLinks` -> `UpdateWorkerSocialLinksCommand` -> `ApiResponse`
- `POST Workers/UpdateMatchingPreferences` -> `UpdateWorkerMatchingPreferencesCommand` -> `ApiResponse`
- `POST Workers/InitProfilePhotoUpload` -> `InitWorkerProfilePhotoUploadCommand` -> `ApiResponse<ObjectStorageUploadInitModel>`
- `POST Workers/ConfirmProfilePhotoUpload` -> `ConfirmWorkerProfilePhotoUploadCommand` -> `ApiResponse`
- `POST Workers/ClearProfilePhoto` -> `ClearWorkerProfilePhotoCommand` -> `ApiResponse`
- `POST Workers/InitCvUpload` -> `InitWorkerCvUploadCommand` -> `ApiResponse<ObjectStorageUploadInitModel>`
- `POST Workers/ConfirmCvUpload` -> `ConfirmWorkerCvUploadCommand` -> `ApiResponse<int>` (upload session id)
- `POST Workers/ConfirmCvReview` -> `ConfirmWorkerCvReviewCommand` -> `ApiResponse`
- `POST Workers/DiscardCvReview` -> `DiscardWorkerCvReviewCommand` -> `ApiResponse`
- `POST Workers/LiveStatus` -> `GetWorkerLiveStatusFeedQuery` (bos body `{}` destekli) -> `ApiResponse<WorkerLiveStatusFeedModel>`
- `POST Workers/NotificationPreview` -> `GetWorkerPersonalizedNotificationPreviewQuery` -> `ApiResponse<WorkerNotificationPreviewModel>`
- `POST Workers/ConfirmPayout` -> `ConfirmWorkerPayoutCommand` -> `ApiResponse`

### Alt kayitlar

- `POST Workers/AddSkill` -> `AddWorkerSkillCommand` -> `ApiResponse<int>`
- `POST Workers/RemoveSkill` -> `RemoveWorkerSkillCommand` -> `ApiResponse`
- `POST Workers/AddAvailability` -> `AddWorkerAvailabilityCommand` -> `ApiResponse<int>`
- `POST Workers/RemoveAvailability` -> `RemoveWorkerAvailabilityCommand` -> `ApiResponse`
- `POST Workers/AddEducation` -> `AddWorkerEducationCommand` -> `ApiResponse<int>`
- `POST Workers/RemoveEducation` -> `RemoveWorkerEducationCommand` -> `ApiResponse`
- `POST Workers/AddExperience` -> `AddWorkerExperienceCommand` -> `ApiResponse<int>`
- `POST Workers/RemoveExperience` -> `RemoveWorkerExperienceCommand` -> `ApiResponse`
- `POST Workers/AddLanguage` -> `AddWorkerLanguageCommand` -> `ApiResponse<int>`
- `POST Workers/RemoveLanguage` -> `RemoveWorkerLanguageCommand` -> `ApiResponse`
- `POST Workers/AddCertificate` -> `AddWorkerCertificateCommand` -> `ApiResponse<int>`
- `POST Workers/RemoveCertificate` -> `RemoveWorkerCertificateCommand` -> `ApiResponse`
- `POST Workers/AddReference` -> `AddWorkerReferenceCommand` -> `ApiResponse<int>`
- `POST Workers/RemoveReference` -> `RemoveWorkerReferenceCommand` -> `ApiResponse`

### Ilan / basvuru / vardiya

- `POST JobPostings/ListOpen` -> `ListOpenJobPostingsQuery` -> `PageableApiResponse<JobPostingSummaryModel>`
- `POST JobPostings/GetById` -> `GetJobPostingByIdQuery` -> `ApiResponse<JobPostingDetailModel>`
- `POST JobPostings/ListSemanticMatched` -> `ListSemanticMatchedJobPostingsQuery` (bos body `{}` destekli) -> `ApiResponse<IReadOnlyList<SemanticMatchedJobPostingModel>>`
- `POST JobApplications/Submit` -> `SubmitJobPostingApplicationCommand` -> `ApiResponse<int>`
- `POST JobApplications/Withdraw` -> `WithdrawJobPostingApplicationCommand` -> `ApiResponse`
- `POST JobApplications/MyApplications` -> `ListMyJobApplicationsQuery` (bos body `{}` destekli) -> `PageableApiResponse<WorkerJobApplicationListItemModel>`
- `POST ShiftAssignments/MyAssignments` -> `ListMyShiftAssignmentsQuery` (bos body `{}` destekli) -> `PageableApiResponse<WorkerShiftAssignmentListItemModel>`
- `POST ShiftAssignments/CheckIn` -> `CheckInShiftAssignmentCommand` -> `ApiResponse`
- `POST ShiftAssignments/CheckOut` -> `CheckOutShiftAssignmentCommand` -> `ApiResponse`

## Employer endpointleri (`employer_id` claim)

### Worker goruntuleme (employer-safe)

- `POST Workers/GetById` -> `GetWorkerByIdQuery` -> `ApiResponse<WorkerEmployerSafeDetailModel>`
- `POST Workers/GetDetail` -> `GetWorkerDetailQuery` -> `ApiResponse<WorkerEmployerSafeFullDetailModel>`
- `POST Workers/RecordEmployerWorkerProfileView` -> `RecordEmployerWorkerProfileViewCommand` -> `ApiResponse<RecordEmployerWorkerProfileViewResultModel>`
- `POST Workers/GetProfilePhotoViewUrl` -> `GetWorkerProfilePhotoViewUrlQuery` -> `ApiResponse<MediaBlobViewUrlModel>`

### Is ilani yonetimi

- `POST JobPostings/Create` -> `CreateJobPostingCommand` -> `ApiResponse<int>`
- `PUT JobPostings/Update` -> `UpdateJobPostingCommand` -> `ApiResponse`
- `POST JobPostings/Publish` -> `PublishJobPostingCommand` -> `ApiResponse`
- `POST JobPostings/Cancel` -> `CancelJobPostingCommand` -> `ApiResponse`
- `POST JobPostings/Complete` -> `CompleteJobPostingCommand` -> `ApiResponse`
- `POST JobPostings/AddSkill` -> `AddJobPostingSkillCommand` -> `ApiResponse<int>`
- `POST JobPostings/RemoveSkill` -> `RemoveJobPostingSkillCommand` -> `ApiResponse`
- `POST JobPostings/ListByEmployer` -> `ListJobPostingsByEmployerIdQuery` (bos body `{}` destekli) -> `PageableApiResponse<JobPostingSummaryModel>`

### Basvuru / vardiya / odeme / komisyon

- `POST JobApplications/List` -> `ListJobApplicationsByJobPostingIdQuery` -> `PageableApiResponse<JobApplicationListItemModel>`
- `POST JobApplications/Accept` -> `AcceptJobPostingApplicationCommand` -> `ApiResponse`
- `POST JobApplications/Reject` -> `RejectJobPostingApplicationCommand` -> `ApiResponse`
- `POST ShiftAssignments/Create` -> `CreateShiftAssignmentCommand` -> `ApiResponse<int>`
- `POST ShiftAssignments/SupervisorCheckIn` -> `SupervisorCheckInShiftAssignmentCommand` -> `ApiResponse`
- `POST Employers/CreateWorkerPayout` -> `CreateWorkerPayoutCommand` -> `ApiResponse<int>`
- `POST Employers/MarkWorkerPayoutAsProcessing` -> `MarkWorkerPayoutAsProcessingCommand` -> `ApiResponse`
- `POST Employers/FailWorkerPayout` -> `FailWorkerPayoutCommand` -> `ApiResponse`
- `POST Employers/RetryWorkerPayout` -> `RetryWorkerPayoutCommand` -> `ApiResponse`
- `POST Employers/GenerateCommissionReceivable` -> `GenerateCommissionReceivableCommand` -> `ApiResponse<int>`
- `POST Employers/SetCommissionPolicy` -> `SetEmployerCommissionRateCommand` -> `ApiResponse`
- `POST Employers/GetCommissionEstimate` -> `GetEmployerCommissionEstimateQuery` -> `ApiResponse<EmployerCommissionEstimateModel>`
- `POST Employers/GetCommissionPolicy` -> `GetEmployerCommissionPolicyQuery` -> `ApiResponse<EmployerCommissionPolicyModel>`
- `POST Employers/GetCommissionReceivableByPeriod` -> `GetCommissionReceivableByPeriodQuery` -> `ApiResponse<CommissionReceivableDetailModel>`
- `POST Employers/ListCommissionReceivables` -> `ListCommissionReceivablesByEmployerQuery` -> `PageableApiResponse<CommissionReceivableListItemModel>`
- `POST Employers/ListCommissionSummaries` -> `ListEmployerCommissionSummariesQuery` -> `ApiResponse<IReadOnlyList<EmployerCommissionListItemModel>>`
- `POST Employers/ExportCommissionPoliciesCsv` -> `ExportEmployerCommissionPoliciesCsvQuery` (bos body `{}` destekli) -> `ApiResponse<EmployerCommissionPolicyExportPackageModel>`

### Employer profil / organizasyon

- `POST Employers/GetById` -> `GetEmployerByIdQuery` -> `ApiResponse<EmployerDetailModel>`
- `POST Employers/GetDetail` -> `GetEmployerDetailQuery` -> `ApiResponse<EmployerFullDetailModel>`
- `POST Employers/AddLocation` -> `AddEmployerLocationCommand` -> `ApiResponse<int>`
- `POST Employers/AddSupervisor` -> `AddEmployerSupervisorCommand` -> `ApiResponse<int>`
- `POST Employers/RemoveSupervisor` -> `RemoveEmployerSupervisorCommand` -> `ApiResponse`
- `POST Employers/UpdateSocialLinks` -> `UpdateEmployerSocialLinksCommand` -> `ApiResponse`
- `POST Employers/InitLogoUpload` -> `InitEmployerLogoUploadCommand` -> `ApiResponse<ObjectStorageUploadInitModel>`
- `POST Employers/ConfirmLogoUpload` -> `ConfirmEmployerLogoUploadCommand` -> `ApiResponse`
- `POST Employers/ClearLogo` -> `ClearEmployerLogoCommand` -> `ApiResponse`
- `POST Employers/GetLogoViewUrl` -> `GetEmployerLogoViewUrlQuery` -> `ApiResponse<MediaBlobViewUrlModel>`

## Admin endpointleri

### Sistem kullanicisi

- `POST SystemUsers/List` -> `ListSystemUsersQuery` -> `PageableApiResponse<SystemUserListItemModel>`
- `POST SystemUsers/Me` -> `GetSystemUserMeQuery` (bos body `{}` destekli) -> `ApiResponse<SystemUserMeModel>`
- `POST SystemUsers/Ban` -> `BanSystemUserCommand` -> `ApiResponse`
- `POST SystemUsers/Suspend` -> `SuspendSystemUserCommand` -> `ApiResponse`
- `POST SystemUsers/Reactivate` -> `ReactivateSystemUserCommand` -> `ApiResponse`
- `POST SystemUsers/ChangePassword` -> `ChangeSystemUserPasswordCommand` -> `ApiResponse`
- `POST SystemUsers/RegisterAdmin` -> `RegisterAdminCommand` -> `ApiResponse<int>`
- `POST SystemUsers/SendNotification` -> `SendSystemUserNotificationCommand` -> `ApiResponse<int>`
- `POST SystemUsers/MyNotifications` -> `ListMyNotificationsQuery` (bos body `{}` destekli) -> `PageableApiResponse<SystemUserNotificationListItemModel>`
- `POST SystemUsers/MarkNotificationAsRead` -> `MarkNotificationAsReadCommand` -> `ApiResponse`
- `POST SystemUsers/MarkAllNotificationsAsRead` -> `MarkAllNotificationsAsReadCommand` (bos body `{}` destekli) -> `ApiResponse`

### Grup / yetki

- `POST SystemUserGroups/List` -> `ListSystemUserGroupsQuery` -> `PageableApiResponse<SystemUserGroupListItemModel>`
- `POST SystemUserGroups/Activate` -> `ActivateSystemUserGroupCommand` -> `ApiResponse`
- `POST SystemUserGroups/Deactivate` -> `DeactivateSystemUserGroupCommand` -> `ApiResponse`
- `POST SystemUserGroups/AddPermission` -> `AddSystemUserGroupPermissionCommand` -> `ApiResponse<int>`

### Admin worker/employer/istatistik

- `POST Workers/List` -> `ListWorkersQuery` -> `PageableApiResponse<WorkerListItemModel>`
- `POST Workers/Delete` -> `DeleteWorkerCommand` -> `ApiResponse`
- `POST Workers/SendNotification` -> `SendWorkerNotificationCommand` -> `ApiResponse<int>`
- `POST Employers/List` -> `ListEmployersQuery` -> `PageableApiResponse<EmployerListItemModel>`
- `POST Employers/Activate` -> `ActivateEmployerCommand` -> `ApiResponse`
- `POST Employers/Ban` -> `BanEmployerCommand` -> `ApiResponse`
- `POST Employers/Suspend` -> `SuspendEmployerCommand` -> `ApiResponse`
- `POST Employers/Delete` -> `DeleteEmployerCommand` -> `ApiResponse`
- `POST Statistics/Overview` -> `GetDashboardStatisticsQuery` (bos body `{}` destekli) -> `ApiResponse<DashboardStatisticsModel>`
- `POST Statistics/OverdueSummary` -> `GetOverdueJobSummaryQuery` (bos body `{}` destekli) -> `ApiResponse<OverdueJobSummaryModel>`
- `POST Statistics/MonetizationSummary` -> `GetMonetizationSummaryQuery` (bos body `{}` destekli) -> `ApiResponse<MonetizationSummaryModel>`
- `POST Statistics/FinancialReconciliationSummary` -> `GetFinancialReconciliationSummaryQuery` (bos body `{}` destekli) -> `ApiResponse<FinancialReconciliationSummaryModel>`
- `POST Statistics/FinancialReconciliationRows` -> `ListFinancialReconciliationRowsQuery` -> `PageableApiResponse<FinancialReconciliationListItemModel>`
- `POST Statistics/ExportOverdueAlarmsCsv` -> `ExportOverdueAlarmsCsvQuery` (bos body `{}` destekli) -> `ApiResponse<OverdueAlarmExportPackageModel>`
- `POST Statistics/ExportSystemUserNotificationDispatchesCsv` -> `ExportSystemUserNotificationDispatchesCsvQuery` (bos body `{}` destekli) -> `ApiResponse<SystemUserNotificationDispatchExportPackageModel>`

## Model parametreleri ve donus icerigi nasil okunur

Her endpoint icin kesin alan listesi bu 3 kaynaktan alinmalidir:

1. Request modeli:
   - `src/Application/**/<RequestModel>.cs`
2. Zorunlu alanlar:
   - `src/Application/**/<RequestModel>Validator.cs`
   - `NotEmpty`, `NotNull`, `GreaterThan`, `Length` vb. kurallar frontend validasyonuna aynen tasinmali.
3. Response modeli:
   - `src/Application/**/<ResponseModel>.cs`
   - `ApiResponse<T>.data` icindeki alanlar frontend state modeline birebir map edilmeli.

## Ornek cagrilar

### 1) Login

`POST /SystemUsers/Login`

```json
{
  "email": "user@example.com",
  "password": "Secret123!"
}
```

Beklenen: `ApiResponse<SystemUserTokenModel>`

### 2) Worker profil ozeti

`POST /Workers/GetSelfSummary`

```json
{}
```

Beklenen: `ApiResponse<WorkerSelfDetailModel>`

### 3) Is ilani guncelleme

`PUT /JobPostings/Update`

```json
{
  "jobPostingId": 123
}
```

Beklenen: `ApiResponse`

## Kritik notlar

- Worker session ile `Workers/GetDetail` cagrilmaz; worker kendi profili icin `GetSelfSummary` veya `GetSelfFullDetail` kullanir.
- Employer session ile worker detayinda `Workers/GetDetail` kullanilir.
- Bos body destekli endpointlerde frontend standardi `{}` gondermek olmalidir.

