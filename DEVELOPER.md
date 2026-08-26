# Professional developer workflow

Lubb Interactive Creator is a native C# WPF application. The repository also
contains optional Python support tooling and a static companion site.

## Claude integration

`Services/ClaudeAssistant.cs` implements the official Anthropic Messages API
behind `Core/IAiAssistant.cs`. It is deliberately opt-in:

- Never commit an API key.
- Store user credentials with the Windows DPAPI credential store.
- Do not put keys in `.lubbproject`, logs, diagnostics, prompts, or analytics.
- Ask the user before sending project or diagnostic data to an external service.
- Use a least-data prompt and redact paths, tokens, and personal information.
- Treat model output as advice, not as an executable command.

The client is not automatically wired into recording or file operations.
Production UI wiring should add explicit Enable, Send, and Delete controls plus
an audit notice showing exactly what data will leave the device.

## VS Code / Visual Studio workflow

Use the C# Dev Kit or Visual Studio with the .NET desktop workload. Recommended
checks before a pull request:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

Keep capture, encoding, audio, integrations, storage, and UI behind interfaces.
Do not report `RECORDING`, `LIVE`, or `CONNECTED` without a confirmed service or
process state. Add tests for state transitions and failure recovery before
connecting real hardware.
