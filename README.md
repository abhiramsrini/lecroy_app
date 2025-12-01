# LeCroy Scope Controller (WinForms)

Lightweight Windows utility to connect to LeCroy oscilloscopes via the ActiveDSO COM control. Provides connection controls, simple command send/receive, a dry-run mode for UI testing without hardware, and an activity log.

## Prerequisites

- Windows with .NET 7 SDK (or newer that supports `net7.0-windows`).
- LeCroy ActiveDSO COM control installed and registered (ProgID: `LeCroy.ActiveDSOCtrl.1`).
- If the scope requires it, network access to the instrument’s IP/hostname.

## Build and run

```sh
cd src/LecroyScopeWinForms
dotnet build
dotnet run
```

To publish a single EXE for 64-bit Windows:

```sh
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
```

If your ActiveDSO control is only 32-bit, publish/rerun as `win-x86` and ensure `Prefer32Bit` stays enabled in the csproj.

## Project structure

- `src/LecroyScopeWinForms/LecroyScopeWinForms.csproj` — WinForms app targeting `net7.0-windows`, 32-bit preferred for COM.
- `src/LecroyScopeWinForms/Program.cs` — Application entry.
- `src/LecroyScopeWinForms/MainForm.*` — UI layout (Designer) and logic; branding strip, connection panel, custom command sender, response viewer, log.
- `src/LecroyScopeWinForms/Scope/` — Scope abstraction (`IScopeClient`) and ActiveDSO implementation (`ActiveDsoClient`).
- `Properties/` — WinForms defaults; add resources/icons here if needed.

## Usage

1) Launch the app.  
2) Enter the oscilloscope IP/hostname.  
3) Optional: enable **Dry run** to simulate without hardware.  
4) Click **Connect**; status updates in the footer of the connection box.  
5) Enter a SCPI/command and click **Send** to write to the scope and read back the response.  
6) Enter a scope-side setup path and click **Load** to recall that setup on the instrument (no upload support).  
7) Activity log shows all events with timestamps.

## Customizing

- Branding: update `CompanyBrand` in `MainForm.cs`; add a logo (e.g., a PictureBox) to the brand panel.
- Commands: extend the UI with preset buttons or command sequences; wire them through `IScopeClient`.
- Features to add: screenshot capture, waveform fetch/export, setup load/apply, presets stored in JSON, better error surfacing, and a log save-to-file option.

## Notes on ActiveDSO interop

- The app creates the COM object via `LeCroy.ActiveDSOCtrl` ProgID. Ensure the control is registered (typically done by the LeCroy software installer).
- The wrapper uses `dynamic` for simplicity; you can replace it with an early-bound COM reference for IntelliSense and stronger typing.
- Basic timeout handling is provided; adjust `ReadString` timeout or add cancellation support around long operations as needed.
