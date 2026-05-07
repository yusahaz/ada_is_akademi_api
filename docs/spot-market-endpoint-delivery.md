# Employer Spot Market - Endpoint Delivery

Bu dokuman frontend ekibinin Spot Market ekranlarini fallback'ten canli API'ye gecirmesi icin hazirlandi.

## 1) Endpoint hazirlik durumu

### A) Dashboard / Badge / Anomaly
- `POST Employers/SpotDashboardSummary` -> **Hazir**
- `POST ShiftAssignments/MyAssignments` -> **Kismi (genisletildi)**
  - eklendi: `workerId`, `anomalyType`, `anomalyDetectedAt`

### B) Operations
- `POST ShiftAssignments/ListHistory` -> **Hazir**
- `POST JobPostings/CreateFromTemplate` -> **Hazirlanacak (opsiyonel)**
  - gecici alternatif: `POST JobPostings/Create`

### C) Intelligence
- `POST Workers/SemanticSearch` -> **Hazir**
- `POST Employers/WorkerPortfolio` -> **Hazir**

### D) Finance
- `POST Employers/ListWorkerPayouts` -> **Hazir**
- `POST Employers/CreateWorkerPayout` -> **Hazir (snapshot doner)**
- `POST Employers/MarkWorkerPayoutAsProcessing` -> **Hazir (snapshot doner)**
- `POST Employers/FailWorkerPayout` -> **Hazir (snapshot doner)**
- `POST Employers/RetryWorkerPayout` -> **Hazir (snapshot doner)**
- `POST Workers/ConfirmPayout` -> **Hazir (snapshot doner)**
- `POST Employers/ListCommissionReceivables` -> **Kismi (genisletildi)**
- `POST Employers/GetCommissionReceivableByPeriod` -> **Kismi (genisletildi)**

### E) Settings
- `POST Employers/ListLocations` -> **Hazir**
- `POST Employers/AddLocation` -> **Hazir**
- `POST Employers/UpdateLocation` -> **Hazir**
- `POST Employers/DeleteLocation` -> **Hazir**
- `POST Employers/ListSupervisors` -> **Hazir**
- `POST Employers/AddSupervisor` -> **Hazir**
- `POST Employers/RemoveSupervisor` -> **Hazir**

### F) Disputes
- `POST Employers/ListDisputes` -> **Hazir** (anomaly/assignment durumundan uretilen operasyonel liste)

## 2) Turev endpoint eslestirmesi

- `Employers/SpotDashboardSummary` yerine gecici: `Statistics/Overview` + `Statistics/OverdueSummary` + `Statistics/FinancialReconciliationSummary`
- `JobPostings/CreateFromTemplate` yerine gecici: `JobPostings/Create`
- `Employers/ListLocations` yerine gecici: `Employers/GetDetail` (`locations`)
- `Employers/ListSupervisors` yerine gecici: `Employers/GetDetail` (`supervisors`)

## 3) Scope bilgisi ve auth

- Tum endpointler auth zorunlu (`ApiControllerBase` => `[Authorize]`).
- Scope enumlari:
  - `EmployerScoped`
  - `LocationScoped`
- `Employers/ListSupervisors` cevabinda:
  - `assignedLocationIds[]`
  - `groupIds[]`
  - `scopeType`

## 4) Ornek request/response payloadlari

### Spot dashboard summary

Request:
```json
{}
```

Response:
```json
{
  "isSuccess": true,
  "message": null,
  "errorCode": null,
  "errors": null,
  "statusCode": 200,
  "data": {
    "dailyFillRatePercent": 62.5,
    "activeWorkerCount": 120,
    "openPostingCount": 18,
    "pendingApplicationCount": 44,
    "activeAnomalyCount": 3,
    "pendingPayoutCount": 9
  }
}
```

### Shift history

Request:
```json
{
  "dateFrom": "2026-05-01T00:00:00Z",
  "dateTo": "2026-05-31T23:59:59Z",
  "locationId": 12,
  "status": "CheckedOut",
  "limit": 20,
  "offset": 0
}
```

Response `data[]` item:
```json
{
  "assignmentId": 501,
  "workerId": 91,
  "status": "CheckedOut",
  "wasNoShow": false,
  "completedAt": "2026-05-04T18:25:11Z",
  "anomalySummary": null,
  "disputeSummary": null
}
```

### Worker semantic search

Request:
```json
{
  "queryText": "kasiyer aksam vardiyasi",
  "locationId": 12,
  "limit": 10,
  "offset": 0
}
```

Response `data[]` item:
```json
{
  "workerId": 91,
  "fullName": "Ada Yilmaz",
  "semanticScore": 86,
  "reliabilityScore": 72,
  "lastWorkedAt": "2026-04-30T19:00:00Z",
  "skills": ["Kasiyerlik", "POS"],
  "languages": ["Turkce", "Ingilizce"],
  "city": "Istanbul"
}
```

### Payout action snapshot

Request (`Employers/MarkWorkerPayoutAsProcessing`):
```json
{
  "workerPayoutId": 7001
}
```

Response:
```json
{
  "isSuccess": true,
  "data": {
    "workerPayoutId": 7001,
    "status": "Processing",
    "isLocked": true,
    "updatedAt": "2026-05-07T09:40:00Z"
  }
}
```

### List supervisors

Response `data[]` item:
```json
{
  "systemUserId": 112,
  "fullName": "Mert Demir",
  "email": "mert.demir@example.com",
  "assignedLocationIds": [12, 14],
  "groupIds": [3, 8],
  "scopeType": "LocationScoped"
}
```

## 5) Enum / status mapping

### ShiftAssignmentStatus
- `Pending`
- `AwaitingMutualQr`
- `CheckedIn`
- `CheckedOut`

### WorkerPayoutStatus
- `Pending`
- `Processing`
- `Paid`
- `Failed`

### MembershipScopeType
- `Global`
- `EmployerScoped`
- `LocationScoped`

### Dispute status (operasyonel feed)
- `InReview`
- `Resolved`

### Commission receivable status (su an)
- `Invoiced` (su an sistemde bu deger uretilir)

## 6) Frontend ekibine verilecek prompt

Backend Spot Market gecisi icin su entegrasyon kurallarini uygula: once endpoint durumunu `Hazir/Kismi/Hazirlanacak` olarak kontrol et. Tum cagrilarda `Authorization: Bearer <token>` gonder, envelope parse'ini `isSuccess`, `message`, `errorCode`, `errors`, `data` ustunden yap. Liste endpointlerinde `limit/offset/totalCount/hasMore` kullan. Dashboard icin birincil endpoint `POST Employers/SpotDashboardSummary`; fallback sadece bu endpoint ulasilamazsa `Statistics/Overview + Statistics/OverdueSummary + Statistics/FinancialReconciliationSummary` kombinasyonu olsun. Payout aksiyonlarinda donen snapshot (`workerPayoutId`, `status`, `isLocked`, `updatedAt`) ile optimistic UI state guncelle. Supervisor filtrelerinde `scopeType`, `assignedLocationIds`, `groupIds` alanlarini birebir map et. Enum degerlerini backend stringleriyle birebir kullan (`ShiftAssignmentStatus`, `WorkerPayoutStatus`, `MembershipScopeType`) ve tarih/saat parse'larini ISO-8601 kabul et.
