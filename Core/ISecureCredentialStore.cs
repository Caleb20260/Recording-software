namespace LubbInteractiveCreator.Core;

public interface ISecureCredentialStore
{
    Task SaveAsync(string key, string secret, CancellationToken cancellationToken = default);
    Task<string?> ReadAsync(string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}