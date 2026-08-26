# Lubb Interactive Creator

Native Windows desktop creator software by Lubb Interactive.

Lubb Interactive Creator is being built as a modular WPF application for recording,
streaming, replay, scene composition, clip editing, media management, Xbox capture,
and Discord integration. Basic project creation and `.lubbproject` persistence are
implemented in the current foundation.

## Current foundation

- Windows desktop target: `net10.0-windows` with WPF and MVVM state binding
- Branded dashboard with Home, Creator, Scenes, Audio, Recording, Streaming, Clips,
  Media, Xbox Hub, and Discord Hub navigation surfaces
- Truthful capability states: unavailable services never claim to be recording, live,
  connected, or saved
- Native Windows save dialog and JSON-backed `.lubbproject` persistence
- `IRecordingEngine` boundary ready for Windows Graphics Capture, Media Foundation,
  WASAPI, and hardware encoder implementations
- Per-user execution manifest (`asInvoker`); no fake elevation or security prompts

## Build

The project must be built and run on Windows to use WPF and device APIs. A Windows
SDK installation and the .NET 10 SDK are required.

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

The Linux development container can compile the Windows target with
`EnableWindowsTargeting`, but cannot launch the WPF executable or validate capture
devices.

## Product identity

**LUBB INTERACTIVE**  
Lubb Interactive Creator  
EST. 2026  
(C) 2026 Lubb Interactive. All Rights Reserved.

Lubb Interactive is an independent product. Xbox and Discord are descriptive
integration targets and are not represented as official partner software.

See [ARCHITECTURE.md](ARCHITECTURE.md) and [SECURITY.md](SECURITY.md) for the
implementation boundaries and security requirements.

## Distribution channels

The project is prepared for three legitimate channels: portable ZIP, signed Inno
Setup installer, and Microsoft Store/MSIX. See [packaging/README.md](packaging/README.md)
for the Store identity, signing, permissions, and certification requirements. The
GitHub workflow at [.github/workflows/release.yml](.github/workflows/release.yml)
builds Windows artifacts when a version tag is pushed.

## Release package

The current release is `0.3.1`. On Windows, run `package-release.ps1` to produce
`artifacts/LubbInteractiveCreator-0.3.1-win-x64.zip`. From Linux or CI, run
`./package-release.sh` when the .NET SDK and `zip` are installed. This is a portable
publish package. If Inno Setup is installed, either script also produces
`artifacts/installer/LubbInteractiveCreatorSetup-0.3.1.exe`, with a visible setup
wizard, progress display, Start Menu shortcut, optional desktop shortcut, upgrade
support, and uninstaller. The installer must be code-signed before production
distribution.

## What the software does

The current release is a secure native WPF desktop recorder foundation. It opens as a native WPF
Windows application, creates projects, saves them as `.lubbproject` files, remembers
the active save location, creates rolling backups, and presents the creator workspace
for recording, streaming, replay, clips, media, Xbox, and Discord. Device capture,
encoding, live streaming, and official integrations are intentionally capability-gated
until their Windows implementations and hardware tests are complete.

## Recording engine

Release `0.3.1` includes a real FFmpeg recording engine. On Windows, install an
FFmpeg build that is licensed for your distribution and put `ffmpeg.exe` on `PATH`,
or enter its full path in the Recording Engine panel. It captures the desktop with
Windows `gdigrab`, encodes H.264/AAC MP4, supports pause/resume/stop, and writes to
the configured output folder. A DirectShow microphone device name can be entered
for audio input. The UI only shows `RECORDING` after FFmpeg starts successfully.
