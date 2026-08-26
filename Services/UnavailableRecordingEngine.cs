using LubbInteractiveCreator.Core;

namespace LubbInteractiveCreator.Services;

public sealed class UnavailableRecordingEngine : IRecordingEngine
{
    public bool IsAvailable => false;
    public bool IsRecording => false;

    public Task StartAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("Windows capture components are not configured yet.");

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}