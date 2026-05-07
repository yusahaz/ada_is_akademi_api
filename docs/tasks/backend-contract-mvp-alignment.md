# Backend contract MVP alignment

## Goal

- Align backend read contracts with `docs/backend-contract-oneri.md` MVP scope without breaking existing endpoint routes.

## Out of scope (for this task file)

- Persistence schema changes that require new migrations.
- Frontend implementation details.
- Non-MVP items (`mobileKind`, profile facade consolidation, KPI phase-2 enrichments).

## Checklist

- [x] Analyze current API/application gaps for contract proposal sections 1-4.
- [x] Start MVP implementation by enriching job posting list summary payload used by `JobPostings/ListOpen` and `JobPostings/ListByEmployer`.
- [x] Enrich worker `MyApplications` payload with job card join fields and check-in snapshot (`AssignmentId` added; QR resolve still pending).
- [ ] Add QR preflight/resolve query endpoint (`ShiftAssignments/ResolveQr`) while preserving existing check-in commands.
- [ ] Provide employer candidates feed model (either extend `JobApplications/List` safely or add dedicated query endpoint).
- [ ] Add/adjust tests for enriched projections and endpoint contracts.

## Notes

- Current implementation started with additive `JobPostingSummaryModel` expansion to support mobile card rendering fields.
