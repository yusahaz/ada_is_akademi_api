# Backend Contract Onerisi (Flutter Uygulamasi Icin)

Bu dokuman, mobil uygulama ekranlarinin tasarimini bozmadan API ile birebir beslenebilmesi icin onerilen endpoint/response sozlesmesini icerir.

## 1) Worker Job Feed (Home/Search/Job Detail)

### Problem
- `JobPostingSummaryModel` ile kart/detay ekrani icin gerekli alanlar tamamlanamiyor.
- Sirket adi, logo URL, lokasyon, etiketler, detay aciklama, sorumluluklar ve gereksinimler eksik.

### Oneri
- Endpoint:
  - `POST /WorkerJobFeed/List` (yeni) veya `POST /JobPostings/ListOpen` response genisletme
- Response item (onerilen):
  - `jobPostingId: number`
  - `title: string`
  - `employer: { id: number, name: string, logoUrl: string | null }`
  - `locationText: string`
  - `workType: "Remote" | "Onsite" | "Hybrid"`
  - `salary: { min: number | null, max: number | null, currency: string }`
  - `tags: string[]`
  - `isFeatured: boolean`
  - `postedAt: string (date-time)`
  - `description: string`
  - `responsibilities: string[]`
  - `requirements: string[]`
  - `applicationCount: number`

## 2) Worker Applications + Card Join Data

### Problem
- `MyApplications` response’i ilan kartini dolduracak alanlari getirmiyor.
- QR/check-in token bilgisi yok.

### Oneri
- Endpoint:
  - `POST /JobApplications/MyApplicationsFeed` (yeni)
- Response item:
  - `applicationId: number`
  - `jobPostingId: number`
  - `status: enum`
  - `appliedAt: string (date-time)`
  - `job: { title: string, employerName: string, employerLogoUrl: string | null, locationText: string }`
  - `checkIn: { qrCodeText: string | null, assignmentId: number | null }`

## 3) Check-in QR Contract

### Problem
- Mobil QR akisi ile API komutlari (`assignmentId`, `checkInTokenHash`) arasinda acik bir kontrat yok.

### Oneri
- QR payload standardi:
  - `ADA_CHECKIN_V1:{ "assignmentId": 123, "token": "raw_or_signed_token" }`
- Dogrulama endpointi:
  - `POST /ShiftAssignments/ResolveQr` (yeni)
- Resolve response:
  - `assignmentId: number`
  - `status: enum`
  - `canCheckIn: boolean`
  - `canCheckOut: boolean`
  - `message: string | null`

## 4) Employer Candidates Feed (Operations)

### Problem
- `JobApplications/List` sadece `workerId/status/appliedAt` veriyor.
- Aday kartlari icin ad, unvan, lokasyon, skor vb. yok.

### Oneri
- Endpoint:
  - `POST /Employers/CandidatesFeed` (yeni) veya `JobApplications/List` response genisletme
- Response item:
  - `applicationId: number`
  - `jobPostingId: number`
  - `worker: { id: number, fullName: string, title: string | null, locationText: string | null, avatarUrl: string | null }`
  - `matchScore: number | null`
  - `status: enum`
  - `appliedAt: string (date-time)`

## 5) Employer Postings KPI Fields

### Problem
- Kurumsal ilan kartlarindaki KPI’lar (`views`, `newToday`, `department`) mevcut modelde eksik.

### Oneri
- `ListByEmployer` item alanlari:
  - `departmentName: string | null`
  - `viewCount: number`
  - `newApplicationsToday: number`
  - `totalApplications: number`

## 6) Worker Profile Full View Model

### Problem
- Profil ekraninin tek modelde ihtiyac duydugu alanlar dağınık.

### Oneri
- Endpoint:
  - `POST /Workers/GetSelfProfileView` (yeni facade endpoint)
- Response:
  - `identity: { fullName, title, locationText, avatarUrl }`
  - `stats: { applications, views, saved }`
  - `cvs: [{ id, fileName, fileSizeKb, updatedAt, isPrimary }]`
  - `skills: [{ id, name, level }]`
  - `experiences: [{ id, company, title, startDate, endDate, description }]`
  - `privacy: {...}`
  - `settings: {...}`

## 7) Notification Type Mapping

### Problem
- Mobilde `application/interview/message/system` tipleri var; API’de dogrudan birebir alan yok.

### Oneri
- `SystemUserNotificationListItemModel` genisletme:
  - `mobileKind: "application" | "interview" | "message" | "system"`
- Alternatif:
  - `templateCode` -> `mobileKind` mapping tablosu backend tarafinda sabitlenip dokumante edilmeli.

## 8) Standart API Envelope Kurallari

Tum yeni/yenilenen endpointlerde asagidaki kurallar korunmali:
- `isSuccess`, `message`, `errors`, `statusCode` alanlari korunur.
- Liste endpointleri paging alanlarini sabit sunar:
  - `data`, `totalCount`, `limit`, `offset`, `hasMore`
- Tüm tarih alanlari `UTC date-time` formatinda doner.
- Null olabilir alanlar acikca `nullable` olarak dokumante edilir.

## 9) Onceliklendirme (MVP -> Faz 2)

### MVP (ilk sprint)
- Worker job feed alan genisletmesi
- MyApplications feed join modeli
- Employer candidates feed
- QR resolve contract

### Faz 2
- Worker profile facade endpoint
- Employer postings KPI genisletmeleri
- Notification mobile kind standardizasyonu
