using System.Collections.ObjectModel;

namespace LubbInteractiveCreator.Core;

public sealed class Scene
{
    public string Name { get; set; } = "Gaming";
    public ObservableCollection<Source> Sources { get; set; } = new();
}