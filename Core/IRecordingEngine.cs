namespace LubbInteractiveCreator.Core;

public interface IRecordingEngine
{
    bool IsAvailable { get; }
    bool IsRecording { get; }
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}