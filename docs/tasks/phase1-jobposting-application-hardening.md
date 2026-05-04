# Faz 1 — Job Posting/Application Hardening (Ada Is Akademi)

## Hedef
- Job posting ve job application komutlarinda actor ownership kontrolunu tutarli hale getirmek.
- Mevcut CQRS davranisini bozmadan API sozlesmesi, DI kayitlari ve test guvencesini guclendirmek.

## Kapsam
- `JobPosting` yazma komutlarinda (`Publish`, `Update`, `Cancel`, `Complete`, `AddSkill`, `RemoveSkill`) isveren eslesmesi.
- `JobApplicationsController` ve `JobPostingsController` icin endpoint/auth matrisi dogrulamasi.
- DI (`ServiceRegister`) ve test kapsaminin hardening kapsamiyla uyumlanmasi.

## Kapsam Disi
- Yeni endpoint eklemek veya mevcut HTTP kontratini kirici sekilde degistirmek.
- Domain aggregate davranislarini yeniden tasarlamak.
- UI/Frontend calismalari.

## Gorevler
- [x] Faz hardening task dosyasini olustur.
- [x] Job posting/application command-query ve controller envanterini netlestir.
- [x] Eksik ownership kontrollerini command handler seviyesinde tamamla.
- [x] DI ve API auth/route sozlesmesini tekrar dogrula.
- [x] Provider parity ve regresyon test checklist'i ile kapanis yap.

## Touch List
- `src/Application/Commands/JobPosting/PublishJobPostingCommand.cs`
- `src/Application/Commands/JobPosting/UpdateJobPostingCommand.cs`
- `src/Application/Commands/JobPosting/CancelJobPostingCommand.cs`
- `src/Application/Commands/JobPosting/CompleteJobPostingCommand.cs`
- `src/Application/Commands/JobPosting/AddJobPostingSkillCommand.cs`
- `src/Application/Commands/JobPosting/RemoveJobPostingSkillCommand.cs`
- `src/Application/DependencyInjection/ServiceRegister.cs`
- `src/Api/Controllers/JobPostingsController.cs`
- `src/Api/Controllers/JobApplicationsController.cs`
- `src/Persistence/Mapping/JobApplicationConfiguration.cs`
- `src/Persistence/Mapping/JobPostingSkillConfiguration.cs`
- `tests/ApplicationTests/Phase1CriticalCommandHandlersTests.cs`

## Endpoint/Auth Matrisi (Dogrulama)
- `JobPostings/GetById`: `AllowAnonymous`
- `JobPostings/ListOpen`: `AllowAnonymous`
- `JobPostings/Create|Update|Publish|Cancel|Complete|AddSkill|RemoveSkill`: `Authorize` + actor `employer_id` yalnizca `IExecutionContext` uzerinden
- `JobPostings/ListByEmployer`: `Authorize` + actor `employer_id` yalnizca `IExecutionContext` uzerinden
- `JobApplications/List|Accept|Reject`: `Authorize` + actor `employer_id`
- `JobApplications/Submit|Withdraw`: `Authorize` + actor `worker_id`

## Persistence/Test Notlari
- `JobApplication(JobPostingId, WorkerId)` unique index mevcut; race riskine karsi uygulama idempotency korunur.
- `JobPostingSkill` benzersizlik su an domain kuralinda; DB-level unique sonraki migration kararina bagli.
- Query tarafinda provider farki notu mevcut (SQLite sira davranisi), test checklist'inde parity adimi tutulur.

## Kabul Kriterleri
- Job posting yazma komutlarinda ownership kontrolu tutarli ve testli.
- Handler/validator kayitlari `ServiceRegister` ile uyumlu.
- API auth beklentisi dokumante ve mevcut davranisla tutarli.
- Done/Follow-ups bolumu guncel.

## Done / Follow-ups
- Done: `Publish`, `Update`, `Cancel`, `Complete`, `AddSkill`, `RemoveSkill` handler'larina `IExecutionContext` uzerinden `employer_id` ownership dogrulamasi eklendi.
- Done: `Phase1CriticalCommandHandlersTests` icine ownership mismatch senaryosu (`PublishJobPostingHandler_throws_when_employer_mismatch`) eklendi ve testler gecti.
- Done: API katmaninda route/auth matrisinin mevcut kurallarla uyumu tekrar dogrulandi (anonymous sadece katalog uc noktalarinda).
- Follow-up: JobPosting yazma komutlari icin actor claim zorunlulugunun API/consumer dokumantasyonuna kisa not dusulebilir.
