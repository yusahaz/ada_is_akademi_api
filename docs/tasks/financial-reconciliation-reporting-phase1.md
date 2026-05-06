# Financial reconciliation reporting - phase 1

## Goal

- Close the first slice of deferred "finansal mutabakat ve ileri raporlama" by adding a read-model summary endpoint.

## Delivered

- [x] `GetFinancialReconciliationSummaryQuery` + validator + handler
- [x] `FinancialReconciliationSummaryModel` and per-currency row model
- [x] Statistics API endpoint `FinancialReconciliationSummary`
- [x] Cache key + dependency wiring (`CommissionReceivableAllDependency`, `WorkerPayoutAllDependency`)
- [x] Build/tests green

## Next slices

- Employer/location/date filtered reconciliation report with `ReportFilter` style payload.
- CSV export for reconciliation rows.
