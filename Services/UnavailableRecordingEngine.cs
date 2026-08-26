using LubbInteractiveCreator.Core;

namespace LubbInteractiveCreator.Services;

public sealed class UnavailableRecordingEngine : IRecordingEngine
{
    public bool IsAvailable => false;
    public bool IsRecording => false;
    public bool IsPaused => false;
    public string? OutputPath => null;
    event EventHandler<string>? IRecordingEngine.StatusChanged { add { } remove { } }

    public Task StartAsync(RecordingSettings settings, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("Windows capture components are not configured yet.");

    public Task PauseAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task ResumeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}