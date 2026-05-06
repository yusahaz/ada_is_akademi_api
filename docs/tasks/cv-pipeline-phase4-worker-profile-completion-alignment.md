# CV pipeline - phase 4 worker profile completion alignment

## Goal

- Align CV review/apply outputs with worker profile completion and self-detail read model consistency in Ada Is Akademi.
- Ensure Domain + Application + Persistence + API layers stay contract-compatible while extending worker profile data quality.

## Out of scope

- Mobile/web UI implementation details.
- Replacing extraction provider strategy or prompt engineering.
- New employer-facing contracts unrelated to worker self profile.

## Scope and dependencies

1. Reconfirm worker profile completion rules and expected weighted fields after CV apply flow.
2. Validate `CvUploadSession` -> worker aggregate write path for deterministic and idempotent behavior.
3. Keep worker self-detail query shape/cache and invalidation strategy coherent after profile mutations.
4. Expose only required worker self API behavior changes (if needed), without breaking existing endpoint contract style.

## Checklist (dependency order)

- [x] Domain: verify worker aggregate write methods used by CV apply path preserve invariants for education/experience/certificate/language/skill rows.
- [x] Domain: document and confirm lifecycle assumptions between `CvUploadSessionStatus` transitions and worker profile writes.
- [ ] Persistence: review mapping/index constraints for `CvUploadSession` and related worker profile entities impacted by apply flow.
- [x] Application: review `ConfirmWorkerCvReviewCommand` apply toggles and ensure deterministic idempotent execution for repeated confirmations.
- [ ] Application: validate `IWorkerProfileCompletionEvaluator` weighting behavior against fields populated by CV apply.
- [ ] Application: verify `GetWorkerSelfDetailQuery` and related models return stable profile completion and category/salary projections.
- [ ] Application: confirm cache keys/dependencies and invalidation calls cover worker profile mutations created by CV review/apply commands.
- [ ] API: verify worker self endpoints continue to use CQRS body contracts and `ApiResponse` envelope without route/query drift.
- [ ] API: if contract change is required, update OpenAPI metadata (`Tags`, `ProducesResponseType`, summaries) consistently.
- [x] Cross-layer: add/update focused tests for CV review confirm -> worker profile data -> self-detail/profile-completion readback path.
- [ ] Documentation: sync completion notes in this task file and mark deferred items explicitly.

## Files and APIs to inspect/update

- `src/Domain/Entities/Worker.cs`
- `src/Domain/Entities/CvUploadSession.cs`
- `src/Application/Commands/Worker/ConfirmWorkerCvReviewCommand*`
- `src/Application/Services/WorkerProfileCompletionEvaluator.cs`
- `src/Application/Queries/Worker/GetWorkerSelfDetailQuery*`
- `src/Application/Queries/Worker/WorkerInterestedJobCategoryItemsReader.cs`
- `src/Persistence/Mapping/CvUploadSessionConfiguration.cs`
- `src/Api/Controllers/WorkersController.cs`

## Validation notes

- Re-run existing tests covering worker profile enrichment and CV pipeline apply behavior.
- Add regression check for cache invalidation + consistent profile completion percentage after confirm flow.

## Done / follow-ups

- [ ] Phase 4 implementation completed and checklist updated.
- [ ] Any deferred items moved to execution tracker with rationale.
