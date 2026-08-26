namespace LubbInteractiveCreator.Core;

public interface IAiAssistant
{
    Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default);
}
