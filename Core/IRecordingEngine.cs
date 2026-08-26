namespace LubbInteractiveCreator.Core;

public interface IRecordingEngine
{
    bool IsAvailable { get; }
    bool IsRecording { get; }
    bool IsPaused { get; }
    string? OutputPath { get; }
    event EventHandler<string>? StatusChanged;
    Task StartAsync(RecordingSettings settings, CancellationToken cancellationToken = default);
    Task PauseAsync(CancellationToken cancellationToken = default);
    Task ResumeAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}