using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using LubbInteractiveCreator.Core;

namespace LubbInteractiveCreator.Services;

public sealed class FfmpegRecordingEngine : IRecordingEngine, IDisposable
{
    private Process? process;
    private RecordingSettings? settings;

    public bool IsAvailable => FindFfmpeg(settings?.FfmpegPath ?? "ffmpeg.exe") is not null;
    public bool IsRecording => process is { HasExited: false };
    public bool IsPaused { get; private set; }
    public string? OutputPath { get; private set; }
    public event EventHandler<string>? StatusChanged;

    public async Task StartAsync(RecordingSettings recordingSettings, CancellationToken cancellationToken = default)
    {
        if (IsRecording) throw new InvalidOperationException("A recording is already active.");
        settings = recordingSettings;
        var executable = FindFfmpeg(settings.FfmpegPath) ?? throw new FileNotFoundException(
            "FFmpeg was not found. Install FFmpeg and set its path in Recording settings.");
        ValidateSettings(settings);

        Directory.CreateDirectory(settings.OutputDirectory);
        OutputPath = Path.Combine(settings.OutputDirectory, $"Recording-{DateTime.Now:yyyyMMdd-HHmmss}.mp4");
        var arguments = BuildArguments(settings, OutputPath);
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                StandardErrorEncoding = Encoding.UTF8
            },
            EnableRaisingEvents = true
        };
        process.ErrorDataReceived += ProcessErrorDataReceived;
        process.Exited += ProcessExited;
        if (!process.Start()) throw new InvalidOperationException("FFmpeg could not be started.");
        process.BeginErrorReadLine();
        IsPaused = false;
        StatusChanged?.Invoke(this, "Recording started.");
        await Task.CompletedTask;
    }

    public async Task PauseAsync(CancellationToken cancellationToken = default)
    {
        if (!IsRecording || IsPaused || process is null) return;
        await process.StandardInput.WriteLineAsync("p");
        await process.StandardInput.FlushAsync(cancellationToken);
        IsPaused = true;
        StatusChanged?.Invoke(this, "Recording paused.");
    }

    public async Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        if (!IsRecording || !IsPaused || process is null) return;
        await process.StandardInput.WriteLineAsync("p");
        await process.StandardInput.FlushAsync(cancellationToken);
        IsPaused = false;
        StatusChanged?.Invoke(this, "Recording resumed.");
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (process is null || process.HasExited) return;
        await process.StandardInput.WriteLineAsync("q");
        await process.StandardInput.FlushAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        if (process.ExitCode != 0)
            throw new InvalidOperationException("FFmpeg stopped with an error. Check the recording device and settings.");
        StatusChanged?.Invoke(this, $"Recording saved to {Path.GetFileName(OutputPath)}.");
    }

    public void Dispose()
    {
        process?.Dispose();
        process = null;
    }

    private void ProcessErrorDataReceived(object sender, DataReceivedEventArgs args)
    {
        if (!string.IsNullOrWhiteSpace(args.Data) && args.Data.Contains("error", StringComparison.OrdinalIgnoreCase))
            StatusChanged?.Invoke(this, "FFmpeg reported an error while recording.");
    }

    private void ProcessExited(object? sender, EventArgs args)
    {
        if (process?.ExitCode is not 0) StatusChanged?.Invoke(this, "Recording stopped unexpectedly.");
    }

    private static string? FindFfmpeg(string configuredPath)
    {
        if (File.Exists(configuredPath)) return configuredPath;
        var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        return path.Split(Path.PathSeparator).Select(directory => Path.Combine(directory, configuredPath))
            .FirstOrDefault(File.Exists);
    }

    private static void ValidateSettings(RecordingSettings settings)
    {
        if (settings.Width < 320 || settings.Height < 240) throw new ArgumentOutOfRangeException(nameof(settings.Width), "Resolution is too small.");
        if (settings.FramesPerSecond is < 1 or > 240) throw new ArgumentOutOfRangeException(nameof(settings.FramesPerSecond), "FPS must be between 1 and 240.");
        if (settings.VideoBitrateKbps < 500) throw new ArgumentOutOfRangeException(nameof(settings.VideoBitrateKbps), "Video bitrate is too low.");
    }

    private static string BuildArguments(RecordingSettings settings, string outputPath)
    {
        var builder = new StringBuilder();
        builder.Append("-y -f gdigrab -framerate ").Append(settings.FramesPerSecond.ToString(CultureInfo.InvariantCulture));
        builder.Append(" -video_size ").Append(settings.Width).Append('x').Append(settings.Height).Append(" -i desktop ");
        if (!string.IsNullOrWhiteSpace(settings.MicrophoneDevice))
            builder.Append("-f dshow -i audio=\"").Append(settings.MicrophoneDevice.Replace("\"", string.Empty)).Append("\" ");
        builder.Append("-c:v libx264 -preset veryfast -pix_fmt yuv420p -b:v ")
            .Append(settings.VideoBitrateKbps).Append("k -c:a aac -b:a 192k -movflags +faststart \"")
            .Append(outputPath.Replace("\"", string.Empty)).Append('"');
        return builder.ToString();
    }
}