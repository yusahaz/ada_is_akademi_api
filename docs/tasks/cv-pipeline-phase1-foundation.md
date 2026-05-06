# CV pipeline - phase 1 foundation

## Goal

- Add the first runnable slice of CV upload pipeline in Ada Is Akademi:
  - worker-scoped presigned upload init,
  - upload confirm that creates `CvUploadSession`,
  - domain lifecycle/state foundation for extraction and review phases.

## Out of scope

- Actual extraction provider integration (`ICvExtractionService` implementation).
- Worker review UI/API for applying extracted fields to profile rows.
- Background job orchestration beyond domain-ready status transitions.

## Phase 1 checklist

- [x] Domain enums: `CvFileFormat`, `CvUploadSessionStatus`
- [x] Domain entity: `CvUploadSession` lifecycle + invariants + error codes
- [x] Persistence mapping + migration
- [x] Application commands: `InitWorkerCvUploadCommand`, `ConfirmWorkerCvUploadCommand`
- [x] API endpoints on `WorkersController`
- [x] DI registrations + validators
- [x] Build/tests green

## Notes

- File-size limit follows PRD: max 10 MB.
- Supported formats: PDF and DOCX.
