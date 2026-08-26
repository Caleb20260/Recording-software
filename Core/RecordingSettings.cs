using System.IO;

namespace LubbInteractiveCreator.Core;

public sealed class RecordingSettings
{
    public string OutputDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Lubb Interactive Creator");
    public string FfmpegPath { get; set; } = "ffmpeg.exe";
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
    public int FramesPerSecond { get; set; } = 60;
    public int VideoBitrateKbps { get; set; } = 16000;
    public string MicrophoneDevice { get; set; } = string.Empty;
}