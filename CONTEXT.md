# Project context

Goal: A lightweight WinForms utility to control LeCroy oscilloscopes via ActiveDSO. MVP covers connect/disconnect, custom command send/receive, dry-run simulation, and logging.

## Tech choices

- WinForms (.NET 7, `net7.0-windows`) for fastest UI delivery; `Prefer32Bit` enabled to align with typical 32-bit ActiveDSO COM registration.
- COM interop via `dynamic` and ProgID lookup (`LeCroy.ActiveDSOCtrl.1` fallback to `LeCroy.ActiveDSOCtrl`).
- `IScopeClient` abstraction to allow dry-run mode and future swapping to other transports (e.g., VISA/TCP).

## Current components

- `MainForm` — Branding header, connection panel (address, dry-run, status, connect/disconnect), custom command sender with response view, setup loader (scope-side path), capture+measurement grid (amplitude/frequency/rise for C1–C4), timestamped activity log. Uses `CompanyBrand` constant for easy branding.
- `ActiveDsoClient` — Wraps ActiveDSO COM object, connect/send/disconnect; supports dry run, setup recall, single-shot capture and measurement read (application object model), and basic timeout/read fallback.
- `IScopeClient` — Interface for scope clients, used by UI for testability.

## Assumptions and constraints

- ActiveDSO COM control is installed/registered on target machines; otherwise the app surfaces an error.
- Network reachability to the scope is managed externally.
- No installer yet; single-file publish suggested for distribution.

## Future enhancements (high level)

- Add screenshot capture and waveform fetch/export (CSV, simple chart preview).
- Preset command sets and setup load/apply (e.g., from JSON).
- Save logs to file and include verbose COM tracing toggle.
- Improved error banners and cancellation for long operations.
- DPI-aware manifest and app icon; optional logo in brand panel.

## Development notes

- For UI-only testing, enable dry-run checkbox to bypass hardware.
- If using an early-bound COM reference instead of `dynamic`, add the COM reference in the project and replace dynamic calls with generated types for IntelliSense.
