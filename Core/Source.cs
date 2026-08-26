namespace LubbInteractiveCreator.Core;

public sealed class Source
{
    public string Name { get; set; } = "Source";
    public string Type { get; set; } = "Display Capture";
    public bool IsVisible { get; set; } = true;
    public bool IsLocked { get; set; }
}