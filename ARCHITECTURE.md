# Architecture

The application is split into capability boundaries so Windows-specific APIs do not
leak into the UI.

| Area | Responsibility |
| --- | --- |
| `ViewModels` | UI state, commands, and user-facing status |
| `Core` | Domain models and service contracts |
| `Services` | Persistence and platform service implementations |
| `Capture` | Windows Graphics Capture and capture-device discovery |
| `Audio` | WASAPI devices, routing, monitoring, and filters |
| `Encoding` | Media Foundation and hardware/software encoders |
| `Streaming` | Provider connections, health, and reconnect policy |
| `Projects` | Scene/source graphs, autosave, backups, and recovery |
| `Integrations` | Official Xbox and Discord workflows only |

The dashboard binds to contracts rather than asserting capability. A service must
report confirmed state before the UI can display `RECORDING`, `LIVE`, `CONNECTED`, or
`SAVED`.

## Planned contracts

`ICaptureEngine`, `IAudioEngine`, `IRecordingEngine`, `IStreamingEngine`,
`IReplayEngine`, `IClipEngine`, `ISceneEngine`, `IMediaLibrary`, `IProjectService`,
`IXboxIntegration`, `IDiscordIntegration`, `IPluginManager`, `IUpdateService`,
`IDiagnosticsService`, and `IPrivacyService` are the intended dependency-injection
boundaries. Implementations will be added behind these contracts as each Windows
feature is validated with real hardware.