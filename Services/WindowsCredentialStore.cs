using System.Security.Cryptography;
using System.Text;
using System.IO;
using LubbInteractiveCreator.Core;

namespace LubbInteractiveCreator.Services;

public sealed class WindowsCredentialStore : ISecureCredentialStore
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("LubbInteractiveCreator.Credential.v1");
    private readonly string storageDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Lubb Interactive", "Creator", "Credentials");

    public async Task SaveAsync(string key, string secret, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(secret);
        Directory.CreateDirectory(storageDirectory);
        var protectedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(secret), Entropy, DataProtectionScope.CurrentUser);
        await File.WriteAllBytesAsync(GetPath(key), protectedBytes, cancellationToken);
    }

    public async Task<string?> ReadAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var path = GetPath(key);
        if (!File.Exists(path)) return null;

        var protectedBytes = await File.ReadAllBytesAsync(path, cancellationToken);
        var secret = ProtectedData.Unprotect(protectedBytes, Entropy, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(secret);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var path = GetPath(key);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    private string GetPath(string key)
    {
        var safeKey = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)));
        return Path.Combine(storageDirectory, $"{safeKey}.dat");
    }
}