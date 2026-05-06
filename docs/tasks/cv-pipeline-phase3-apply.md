# CV pipeline - phase 3 apply confirmed payload

## Goal

- Apply confirmed extraction payload into worker profile sections with idempotent domain `Add*` methods.

## Checklist

- [x] `ConfirmWorkerCvReviewCommand` receives granular apply toggles
- [x] Parsed extraction payload model for education/experience/certificate/language/skill rows
- [x] Worker profile writes during confirm flow + cache invalidation
- [x] Build/tests green

## Notes

- Placeholder extractor currently emits deterministic JSON and phase 3 apply logic accepts partial/missing rows safely.
- Unsupported or malformed rows are skipped; domain invariants still enforced by aggregate methods.
