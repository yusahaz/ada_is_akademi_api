# Frontend Endpoint Model Catalog

Bu dosya endpoint map icindeki request/response modellerinin alan bazli dokumantasyonudur.

- Kaynak endpoint listesi: `docs/frontend-endpoint-map.md`
- Not: Zorunlu alanlar validator kurallarindan okunur; nullability tek basina zorunluluk anlamina gelmez.

## AcceptJobPostingApplicationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/AcceptJobPostingApplicationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| ApplicationId | `int` | Hayir |
| JobPostingId | `int` | Hayir |

## ActivateEmployerCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/ActivateEmployerCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |

## ActivateSystemUserGroupCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Authorization/ActivateSystemUserGroupCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SystemUserGroupId | `int` | Hayir |

## AddEmployerLocationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/AddEmployerLocationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| City | `string` | Hayir |
| Description | `string?` | Evet |
| GeofenceRadiusMetres | `int` | Hayir |
| Country | `string` | Hayir |
| Latitude | `double` | Hayir |
| Line1 | `string` | Hayir |
| Longitude | `double` | Hayir |
| Name | `string` | Hayir |

## AddEmployerSupervisorCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/AddEmployerSupervisorCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| LocationId | `int?` | Evet |
| SystemUserId | `int` | Hayir |

## AddJobPostingSkillCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/AddJobPostingSkillCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| IsRequired | `bool` | Hayir |
| JobPostingId | `int` | Hayir |
| Tag | `string` | Hayir |

## AddSystemUserGroupPermissionCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Authorization/AddSystemUserGroupPermissionCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Effect | `PermissionEffect` | Hayir |
| PermissionId | `int` | Hayir |
| SystemUserGroupId | `int` | Hayir |

## AddWorkerAvailabilityCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/AddWorkerAvailabilityCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| DayOfWeek | `DayOfWeek` | Hayir |
| TimeFrom | `TimeOnly` | Hayir |
| TimeTo | `TimeOnly` | Hayir |

## AddWorkerCertificateCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/AddWorkerCertificateCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| DocumentUrl | `string?` | Evet |
| ExpiresAt | `DateOnly?` | Evet |
| IssuedAt | `DateOnly` | Hayir |
| IssuingOrganization | `string` | Hayir |
| Name | `string` | Hayir |

## AddWorkerEducationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/AddWorkerEducationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Department | `string` | Hayir |
| EducationType | `EducationType` | Hayir |
| EndYear | `int?` | Evet |
| IsOngoing | `bool` | Hayir |
| School | `string` | Hayir |
| StartYear | `int` | Hayir |

## AddWorkerExperienceCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/AddWorkerExperienceCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Description | `string?` | Evet |
| EndDate | `DateOnly?` | Evet |
| CompanyName | `string` | Hayir |
| Position | `string` | Hayir |
| StartDate | `DateOnly` | Hayir |

## AddWorkerLanguageCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/AddWorkerLanguageCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Language | `string` | Hayir |
| Level | `LanguageLevel` | Hayir |

## AddWorkerReferenceCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/AddWorkerReferenceCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Company | `string` | Hayir |
| ContactEmail | `string` | Hayir |
| ContactFirstName | `string` | Hayir |
| ContactLastName | `string` | Hayir |
| ContactPhone | `string?` | Evet |
| Position | `string` | Hayir |

## AddWorkerSkillCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/AddWorkerSkillCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Tag | `string` | Hayir |
| WorkerId | `int` | Hayir |

## BanEmployerCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/BanEmployerCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |

## BanSystemUserCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/BanSystemUserCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SystemUserId | `int` | Hayir |

## CancelJobPostingCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/CancelJobPostingCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| JobPostingId | `int` | Hayir |

## ChangeSystemUserPasswordCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/ChangeSystemUserPasswordCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Password | `string` | Hayir |
| SystemUserId | `int` | Hayir |

## CheckInShiftAssignmentCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Assignment/CheckInShiftAssignmentCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| AssignmentId | `int` | Hayir |
| CheckInTokenHash | `string` | Hayir |

## CheckOutShiftAssignmentCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Assignment/CheckOutShiftAssignmentCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| AssignmentId | `int` | Hayir |

## CommissionReceivableDetailModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/CommissionReceivableDetailModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## CommissionReceivableListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/CommissionReceivableListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## CompleteJobPostingCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/CompleteJobPostingCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| JobPostingId | `int` | Hayir |

## ConfirmWorkerPayoutCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/ConfirmWorkerPayoutCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| WorkerPayoutId | `int` | Hayir |

## CreateJobPostingCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/CreateJobPostingCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Description | `string` | Hayir |
| EmployerLocationId | `int` | Hayir |
| HeadCount | `int` | Hayir |
| JobCategoryId | `int` | Hayir |
| ShiftDate | `DateOnly` | Hayir |
| ShiftEndTime | `TimeOnly` | Hayir |
| ShiftStartTime | `TimeOnly` | Hayir |
| Title | `string` | Hayir |
| WageAmount | `decimal` | Hayir |
| WageCurrency | `string` | Hayir |

## CreateShiftAssignmentCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Assignment/CreateShiftAssignmentCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| CheckInTokenHash | `string` | Hayir |
| SupervisorCheckInTokenHash | `string` | Hayir |
| JobApplicationId | `int` | Hayir |

## CreateWorkerPayoutCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/CreateWorkerPayoutCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| AssignmentId | `int` | Hayir |

## DashboardStatisticsModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/DashboardStatisticsModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## DeactivateSystemUserGroupCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Authorization/DeactivateSystemUserGroupCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SystemUserGroupId | `int` | Hayir |

## DeleteEmployerCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/DeleteEmployerCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |

## DeleteWorkerCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/DeleteWorkerCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| WorkerId | `int` | Hayir |

## EmployerCommissionEstimateModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/EmployerCommissionEstimateModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## EmployerCommissionPolicyExportPackageModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/EmployerCommissionPolicyExportPackageModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## EmployerCommissionPolicyModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/EmployerCommissionPolicyModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## EmployerDetailModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/EmployerDetailModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## EmployerFullDetailModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/EmployerFullDetailModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## EmployerListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/EmployerListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## ExportEmployerCommissionPoliciesCsvQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/ExportEmployerCommissionPoliciesCsvQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## ExportOverdueAlarmsCsvQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/ExportOverdueAlarmsCsvQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## ExportSystemUserNotificationDispatchesCsvQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/ExportSystemUserNotificationDispatchesCsvQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## FailWorkerPayoutCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/FailWorkerPayoutCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Reason | `string?` | Evet |
| WorkerPayoutId | `int` | Hayir |

## FinancialReconciliationListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/FinancialReconciliationListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## FinancialReconciliationSummaryModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/FinancialReconciliationSummaryModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GenerateCommissionReceivableCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/GenerateCommissionReceivableCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Amount | `decimal` | Hayir |
| Currency | `string` | Hayir |
| EmployerId | `int` | Hayir |
| PeriodEnd | `DateOnly` | Hayir |
| PeriodStart | `DateOnly` | Hayir |

## GetCommissionReceivableByPeriodQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/GetCommissionReceivableByPeriodQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |
| PeriodEnd | `DateOnly` | Hayir |
| PeriodStart | `DateOnly` | Hayir |

## GetDashboardStatisticsQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/GetDashboardStatisticsQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GetEmployerByIdQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/GetEmployerByIdQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |

## GetEmployerCommissionEstimateQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/GetEmployerCommissionEstimateQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |

## GetEmployerCommissionPolicyQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/GetEmployerCommissionPolicyQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |

## GetEmployerDetailQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/GetEmployerDetailQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |

## GetEmployerLogoViewUrlQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/GetEmployerLogoViewUrlQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GetFinancialReconciliationSummaryQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/GetFinancialReconciliationSummaryQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GetJobPostingByIdQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobPosting/GetJobPostingByIdQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| JobPostingId | `int` | Hayir |

## GetMonetizationSummaryQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/GetMonetizationSummaryQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GetOverdueJobSummaryQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/GetOverdueJobSummaryQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GetSystemUserMeQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/SystemUser/GetSystemUserMeQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GetWorkerByIdQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/GetWorkerByIdQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| WorkerId | `int` | Hayir |

## GetWorkerDetailQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/GetWorkerDetailQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| WorkerId | `int` | Hayir |

## GetWorkerLiveStatusFeedQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/GetWorkerLiveStatusFeedQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Limit | `int` | Hayir |

## GetWorkerPersonalizedNotificationPreviewQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/GetWorkerPersonalizedNotificationPreviewQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| JobPostingId | `int` | Hayir |

## GetWorkerProfilePhotoViewUrlQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/GetWorkerProfilePhotoViewUrlQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GetWorkerSelfDetailQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/GetWorkerSelfDetailQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## GetWorkerSelfFullDetailQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/GetWorkerSelfFullDetailQuery.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## JobApplicationListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobApplication/JobApplicationListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## JobPostingDetailModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobPosting/JobPostingDetailModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## JobPostingSummaryModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobPosting/JobPostingSummaryModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## ListCommissionReceivablesByEmployerQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/ListCommissionReceivablesByEmployerQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |

## ListEmployerCommissionSummariesQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/ListEmployerCommissionSummariesQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Limit | `int` | Hayir |

## ListEmployersQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Employer/ListEmployersQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| CommissionRateMax | `decimal?` | Evet |
| CommissionRateMin | `decimal?` | Evet |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |
| SearchText | `string?` | Evet |
| Status | `EmployerStatus?` | Evet |

## ListFinancialReconciliationRowsQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/ListFinancialReconciliationRowsQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int?` | Evet |
| From | `DateTimeOffset?` | Evet |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |
| To | `DateTimeOffset?` | Evet |

## ListJobApplicationsByJobPostingIdQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobApplication/ListJobApplicationsByJobPostingIdQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| JobPostingId | `int` | Hayir |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |

## ListJobPostingsByEmployerIdQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobPosting/ListJobPostingsByEmployerIdQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |

## ListMyJobApplicationsQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobApplication/ListMyJobApplicationsQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |

## ListMyNotificationsQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/SystemUser/ListMyNotificationsQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| IsRead | `bool?` | Evet |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |

## ListMyShiftAssignmentsQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Assignment/ListMyShiftAssignmentsQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |

## ListOpenJobPostingsQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobPosting/ListOpenJobPostingsQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| CountryCode | `string?` | Evet |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |

## ListSemanticMatchedJobPostingsQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobPosting/ListSemanticMatchedJobPostingsQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Limit | `int` | Hayir |
| WorkerId | `int` | Hayir |

## ListSystemUserGroupsQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/SystemUserGroup/ListSystemUserGroupsQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| IsActive | `bool?` | Evet |
| IsSystem | `bool?` | Evet |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |
| SearchName | `string?` | Evet |

## ListSystemUsersQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/SystemUser/ListSystemUsersQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| AccountStatus | `AccountStatus?` | Evet |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |
| SearchEmail | `string?` | Evet |
| Type | `SystemUserType?` | Evet |

## ListWorkersQuery

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/ListWorkersQuery.cs`

| Alan | Tip | Nullable |
|---|---|---|
| AccountStatus | `AccountStatus?` | Evet |
| Limit | `int` | Hayir |
| Offset | `int` | Hayir |
| SearchEmail | `string?` | Evet |

## LoginSystemUserCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/LoginSystemUserCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| DeviceIdentifier | `string` | Hayir |
| DeviceToken | `string?` | Evet |
| Email | `string` | Hayir |
| Password | `string` | Hayir |
| Platform | `DevicePlatform` | Hayir |

## LogoutSystemUserCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/LogoutSystemUserCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| DeviceIdentifier | `string` | Hayir |
| RefreshToken | `string` | Hayir |
| SystemUserId | `int` | Hayir |

## MarkAllNotificationsAsReadCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/MarkAllNotificationsAsReadCommand.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## MarkNotificationAsReadCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/MarkNotificationAsReadCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| NotificationId | `int` | Hayir |

## MarkWorkerPayoutAsProcessingCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/MarkWorkerPayoutAsProcessingCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| WorkerPayoutId | `int` | Hayir |

## MonetizationSummaryModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/MonetizationSummaryModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## OverdueAlarmExportPackageModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/OverdueAlarmExportPackageModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## OverdueJobSummaryModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/OverdueJobSummaryModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## PublishJobPostingCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/PublishJobPostingCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| JobPostingId | `int` | Hayir |

## ReactivateSystemUserCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/ReactivateSystemUserCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SystemUserId | `int` | Hayir |

## RecordEmployerWorkerProfileViewCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RecordEmployerWorkerProfileViewCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| WorkerId | `int` | Hayir |

## RecordEmployerWorkerProfileViewResultModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RecordEmployerWorkerProfileViewResultModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## RefreshSystemUserTokenCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/RefreshSystemUserTokenCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| DeviceIdentifier | `string` | Hayir |
| RefreshToken | `string` | Hayir |
| SystemUserId | `int` | Hayir |

## RegisterAdminCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/RegisterAdminCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Email | `string` | Hayir |
| FirstName | `string?` | Evet |
| LastName | `string?` | Evet |
| Password | `string` | Hayir |
| Phone | `string?` | Evet |

## RegisterEmployerCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/RegisterEmployerCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerDescription | `string?` | Evet |
| EmployerName | `string` | Hayir |
| EmployerAddressCity | `string` | Hayir |
| EmployerAddressCountry | `string` | Hayir |
| EmployerAddressLine1 | `string` | Hayir |
| EmployerTaxNumber | `string` | Hayir |
| Email | `string` | Hayir |
| FirstName | `string` | Hayir |
| LastName | `string` | Hayir |
| Password | `string` | Hayir |
| Phone | `string` | Hayir |

## RegisterWorkerCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/RegisterWorkerCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Email | `string` | Hayir |
| FirstName | `string?` | Evet |
| LastName | `string?` | Evet |
| Password | `string` | Hayir |
| Phone | `string?` | Evet |

## RejectJobPostingApplicationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/RejectJobPostingApplicationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| ApplicationId | `int` | Hayir |
| JobPostingId | `int` | Hayir |
| Reason | `string?` | Evet |

## RemoveEmployerSupervisorCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/RemoveEmployerSupervisorCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SystemUserId | `int` | Hayir |

## RemoveJobPostingSkillCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/RemoveJobPostingSkillCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| JobPostingId | `int` | Hayir |
| SkillId | `int` | Hayir |

## RemoveWorkerAvailabilityCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RemoveWorkerAvailabilityCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| AvailabilityId | `int` | Hayir |

## RemoveWorkerCertificateCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RemoveWorkerCertificateCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| CertificateId | `int` | Hayir |

## RemoveWorkerEducationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RemoveWorkerEducationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EducationId | `int` | Hayir |

## RemoveWorkerExperienceCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RemoveWorkerExperienceCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| ExperienceId | `int` | Hayir |

## RemoveWorkerLanguageCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RemoveWorkerLanguageCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| LanguageId | `int` | Hayir |

## RemoveWorkerReferenceCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RemoveWorkerReferenceCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| ReferenceId | `int` | Hayir |

## RemoveWorkerSkillCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/RemoveWorkerSkillCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SkillId | `int` | Hayir |

## RequestSystemUserEmailVerificationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/RequestSystemUserEmailVerificationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| ExpiresAt | `DateTimeOffset` | Hayir |
| SystemUserId | `int` | Hayir |
| TokenHash | `string` | Hayir |

## RetryWorkerPayoutCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/RetryWorkerPayoutCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| WorkerPayoutId | `int` | Hayir |

## SendSystemUserNotificationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/SendSystemUserNotificationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Body | `string` | Hayir |
| JobPostingId | `int?` | Evet |
| SystemUserId | `int` | Hayir |
| TemplateCode | `string` | Hayir |
| Title | `string` | Hayir |

## SendWorkerNotificationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/SendWorkerNotificationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Body | `string` | Hayir |
| JobPostingId | `int?` | Evet |
| TemplateCode | `string` | Hayir |
| Title | `string` | Hayir |
| WorkerId | `int` | Hayir |

## SetEmployerCommissionRateCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/SetEmployerCommissionRateCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |
| CommissionRate | `decimal` | Hayir |

## SubmitJobPostingApplicationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobApplication/SubmitJobPostingApplicationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| HasConflictingShift | `bool` | Hayir |
| JobPostingId | `int` | Hayir |
| Note | `string?` | Evet |

## SupervisorCheckInShiftAssignmentCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Assignment/SupervisorCheckInShiftAssignmentCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| AssignmentId | `int` | Hayir |
| SupervisorCheckInTokenHash | `string` | Hayir |

## SuspendEmployerCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/SuspendEmployerCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| EmployerId | `int` | Hayir |

## SuspendSystemUserCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/SuspendSystemUserCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SystemUserId | `int` | Hayir |

## SystemUserGroupListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/SystemUserGroup/SystemUserGroupListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## SystemUserListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/SystemUser/SystemUserListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## SystemUserMeModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/SystemUser/SystemUserMeModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## SystemUserNotificationDispatchExportPackageModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Statistics/SystemUserNotificationDispatchExportPackageModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## SystemUserNotificationListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/SystemUser/SystemUserNotificationListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## SystemUserTokenModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/SystemUserTokenModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## UpdateEmployerSocialLinksCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Employer/UpdateEmployerSocialLinksCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Platform | `SocialMediaPlatform` | Hayir |
| Url | `string?` | Evet |
| Links | `List<EmployerSocialLinkUpdateItem>` | Hayir |

## UpdateJobPostingCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobPosting/UpdateJobPostingCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Description | `string` | Hayir |
| HeadCount | `int` | Hayir |
| JobPostingId | `int` | Hayir |
| ShiftDate | `DateOnly` | Hayir |
| ShiftEndTime | `TimeOnly` | Hayir |
| ShiftStartTime | `TimeOnly` | Hayir |
| Title | `string` | Hayir |
| WageAmount | `decimal` | Hayir |
| WageCurrency | `string` | Hayir |

## UpdateWorkerBioCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/UpdateWorkerBioCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Bio | `string?` | Evet |

## UpdateWorkerMatchingPreferencesCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/UpdateWorkerMatchingPreferencesCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SetExpectedSalary | `bool` | Hayir |
| ExpectedSalaryMinAmount | `decimal?` | Evet |
| ExpectedSalaryMinCurrency | `string?` | Evet |
| ExpectedSalaryMaxAmount | `decimal?` | Evet |
| ExpectedSalaryMaxCurrency | `string?` | Evet |
| SetInterestedJobCategories | `bool` | Hayir |
| InterestedJobCategoryIds | `List<int>?` | Evet |

## UpdateWorkerProfileCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/UpdateWorkerProfileCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| FirstName | `string?` | Evet |
| LastName | `string?` | Evet |
| Nationality | `string?` | Evet |
| University | `string?` | Evet |

## UpdateWorkerSocialLinksCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/Worker/UpdateWorkerSocialLinksCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| Platform | `SocialMediaPlatform` | Hayir |
| Url | `string?` | Evet |
| Links | `List<WorkerSocialLinkUpdateItem>` | Hayir |

## VerifySystemUserEmailCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/SystemUser/VerifySystemUserEmailCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| SystemUserId | `int` | Hayir |
| TokenHash | `string` | Hayir |

## WithdrawJobPostingApplicationCommand

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Commands/JobApplication/WithdrawJobPostingApplicationCommand.cs`

| Alan | Tip | Nullable |
|---|---|---|
| ApplicationId | `int` | Hayir |
| JobPostingId | `int` | Hayir |

## WorkerEmployerSafeDetailModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/WorkerEmployerSafeDetailModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## WorkerEmployerSafeFullDetailModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/WorkerEmployerSafeFullDetailModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## WorkerJobApplicationListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/JobApplication/WorkerJobApplicationListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## WorkerListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/WorkerListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## WorkerLiveStatusFeedModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/WorkerLiveStatusFeedModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## WorkerNotificationPreviewModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/WorkerNotificationPreviewModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## WorkerSelfDetailModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/WorkerSelfDetailModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## WorkerSelfFullDetailModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Worker/WorkerSelfFullDetailModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

## WorkerShiftAssignmentListItemModel

- Kaynak: `C:/WorkingFolder/Azoxia/AdaIsAkademi/src/Application/Queries/Assignment/WorkerShiftAssignmentListItemModel.cs`

- Alanlar: Bu modelde public property bulunamadi (record ctor veya ozel pattern olabilir).

