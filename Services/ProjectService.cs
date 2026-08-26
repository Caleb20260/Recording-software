using System.Text.Json;
using System.IO;
using Microsoft.Win32;
using LubbInteractiveCreator.Core;

namespace LubbInteractiveCreator.Services;

public sealed class ProjectService : IProjectService
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public async Task<string?> SaveAsync(Project project, string? path = null)
    {
        path ??= ChooseSavePath();
        if (path is null) return null;

        project.UpdatedAt = DateTime.UtcNow;
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory))
            throw new IOException("The selected project path has no valid directory.");

        Directory.CreateDirectory(directory);
        RotateBackups(path);
        var temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using var stream = File.Create(temporaryPath);
            await JsonSerializer.SerializeAsync(stream, project, Options);
            await stream.FlushAsync();
            File.Move(temporaryPath, path, true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
        return path;
    }

    public async Task<Project?> OpenAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<Project>(stream, Options);
    }

    private static string? ChooseSavePath()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Lubb project (*.lubbproject)|*.lubbproject",
            DefaultExt = ".lubbproject",
            AddExtension = true,
            FileName = "Untitled.lubbproject"
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    private static void RotateBackups(string path)
    {
        if (!File.Exists(path)) return;

        for (var index = 2; index >= 1; index--)
        {
            var source = index == 1 ? $"{path}.backup" : $"{path}.backup.{index - 1}";
            var destination = $"{path}.backup.{index}";
            if (File.Exists(source)) File.Move(source, destination, true);
        }

        File.Copy(path, $"{path}.backup", true);
    }
}