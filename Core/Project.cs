namespace LubbInteractiveCreator.Core;

public sealed class Project
{
    public string Name { get; set; } = "Untitled Project";
    public string Profile { get; set; } = "Default Profile";
    public string Scene { get; set; } = "Gaming";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}