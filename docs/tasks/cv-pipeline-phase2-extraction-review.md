# CV pipeline - phase 2 extraction and review

## Goal

- Add extraction orchestration and worker review lifecycle on top of phase 1 upload session foundation.

## Scope

- `RunCvExtractionSweepCommand` + Hangfire recurring job wiring.
- `ICvExtractionService` abstraction and deterministic placeholder implementation.
- Worker review commands for session confirm/discard.

## Checklist

- [x] Extraction service abstraction in Application
- [x] Infrastructure placeholder extractor registration
- [x] Automation command + recurring job
- [x] Worker review commands + validators + API endpoints
- [x] Build/tests green

## Next

- Apply confirmed extraction payload to worker profile sections (educations/experiences/certificates/languages/skills).
