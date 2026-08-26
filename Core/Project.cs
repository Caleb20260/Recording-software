using System.Collections.ObjectModel;

namespace LubbInteractiveCreator.Core;

public sealed class Project
{
    public string Name { get; set; } = "Untitled Project";
    public string Profile { get; set; } = "Default Profile";
    public string Scene { get; set; } = "Gaming";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ObservableCollection<Scene> Scenes { get; set; } = new()
    {
        new Scene
        {
            Name = "Gaming",
            Sources = new ObservableCollection<Source>
            {
                new() { Name = "Game Capture", Type = "Game Capture" },
                new() { Name = "Microphone", Type = "Microphone" }
            }
        }
    };
}