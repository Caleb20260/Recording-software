using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LubbInteractiveCreator.Core;
using LubbInteractiveCreator.Services;

namespace LubbInteractiveCreator.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly IProjectService projectService = new ProjectService();
    private Project project = new();
    private string notice = "Ready to create your first project.";
    private string? projectPath;
    private bool analyticsEnabled;
    private bool crashReportsEnabled;
    private Scene? selectedScene;
    private string sceneNameDraft = "Gaming";

    public event PropertyChangedEventHandler? PropertyChanged;
    public Project Project { get => project; private set { project = value; OnPropertyChanged(); OnPropertyChanged(nameof(ProjectName)); } }
    public string ProjectName => Project.Name;
    public string Notice { get => notice; private set { notice = value; OnPropertyChanged(); } }
    public string RecordingStatus => "READY";
    public string StreamingStatus => "OFFLINE";
    public string ReplayStatus => "NOT CONFIGURED";
    public string XboxStatus => "OFFLINE";
    public string DiscordStatus => "DISCONNECTED";
    public string SaveStatus => projectPath is null ? "Not saved" : $"Saved: {Path.GetFileName(projectPath)}";
    public SecurityState Security => new("PROTECTED", "RESTRICTED", "DPAPI", "SIGNED UPDATES", analyticsEnabled, crashReportsEnabled);
    public bool AnalyticsEnabled { get => analyticsEnabled; set { analyticsEnabled = value; OnPropertyChanged(); OnPropertyChanged(nameof(Security)); } }
    public bool CrashReportsEnabled { get => crashReportsEnabled; set { crashReportsEnabled = value; OnPropertyChanged(); OnPropertyChanged(nameof(Security)); } }
    public ObservableCollection<Scene> Scenes => Project.Scenes;
    public Scene? SelectedScene { get => selectedScene; set { selectedScene = value; sceneNameDraft = value?.Name ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(SceneNameDraft)); OnPropertyChanged(nameof(SourceCount)); } }
    public string SceneNameDraft { get => sceneNameDraft; set { sceneNameDraft = value; OnPropertyChanged(); } }
    public int SourceCount => SelectedScene?.Sources.Count ?? 0;

    public ICommand NewProjectCommand { get; }
    public ICommand SaveProjectCommand { get; }
    public ICommand StartRecordingCommand { get; }
    public ICommand GoLiveCommand { get; }
    public ICommand ConnectXboxCommand { get; }
    public ICommand ConnectDiscordCommand { get; }
    public ICommand SaveReplayCommand { get; }
    public ICommand CreateClipCommand { get; }
    public ICommand PrivacyCommand { get; }
    public ICommand NewSceneCommand { get; }
    public ICommand DuplicateSceneCommand { get; }
    public ICommand RenameSceneCommand { get; }
    public ICommand DeleteSceneCommand { get; }
    public ICommand AddSourceCommand { get; }

    public MainViewModel()
    {
        NewProjectCommand = new RelayCommand(NewProject);
        SaveProjectCommand = new AsyncRelayCommand(SaveProjectAsync);
        StartRecordingCommand = new RelayCommand(() => Notice = "Recording is unavailable until a Windows capture device and encoder are configured.");
        GoLiveCommand = new RelayCommand(() => Notice = "Streaming is unavailable until a stream provider and secure credentials are configured.");
        ConnectXboxCommand = new RelayCommand(() => Notice = "Xbox integration is optional and requires an approved capture-device workflow.");
        ConnectDiscordCommand = new RelayCommand(() => Notice = "Discord uses official OAuth and is not connected.");
        SaveReplayCommand = new RelayCommand(() => Notice = "Replay buffer is unavailable until a capture engine is configured.");
        CreateClipCommand = new RelayCommand(() => Notice = "Clip creation requires a recorded source file.");
        PrivacyCommand = new RelayCommand(() => Notice = "Privacy settings are local by default. Optional analytics and crash reports are disabled.");
        NewSceneCommand = new RelayCommand(NewScene);
        DuplicateSceneCommand = new RelayCommand(DuplicateScene);
        RenameSceneCommand = new RelayCommand(RenameScene);
        DeleteSceneCommand = new RelayCommand(DeleteScene);
        AddSourceCommand = new RelayCommand(parameter => AddSource(parameter as string ?? "Source"));
        SelectedScene = Project.Scenes.FirstOrDefault();
    }

    private void NewProject()
    {
        Project = new Project();
        projectPath = null;
        SelectedScene = Project.Scenes.FirstOrDefault();
        OnPropertyChanged(nameof(SaveStatus));
        Notice = "New project created. Save it to begin building your workspace.";
    }

    private void NewScene()
    {
        var scene = new Scene { Name = GetUniqueSceneName("New Scene") };
        Project.Scenes.Add(scene);
        SelectedScene = scene;
        Notice = $"Scene '{scene.Name}' created.";
    }

    private void DuplicateScene()
    {
        if (SelectedScene is null) return;
        var copy = new Scene
        {
            Name = GetUniqueSceneName($"{SelectedScene.Name} Copy"),
            Sources = new ObservableCollection<Source>(SelectedScene.Sources.Select(source => new Source
            {
                Name = source.Name,
                Type = source.Type,
                IsVisible = source.IsVisible,
                IsLocked = source.IsLocked
            }))
        };
        Project.Scenes.Add(copy);
        SelectedScene = copy;
        Notice = $"Scene '{copy.Name}' duplicated.";
    }

    private void RenameScene()
    {
        if (SelectedScene is null || string.IsNullOrWhiteSpace(SceneNameDraft)) return;
        SelectedScene.Name = SceneNameDraft.Trim();
        OnPropertyChanged(nameof(SelectedScene));
        Notice = $"Scene renamed to '{SelectedScene.Name}'. Save the project to keep this change.";
    }

    private void DeleteScene()
    {
        if (SelectedScene is null || Project.Scenes.Count <= 1)
        {
            Notice = "A project must keep at least one scene.";
            return;
        }
        var removedName = SelectedScene.Name;
        var index = Project.Scenes.IndexOf(SelectedScene);
        Project.Scenes.Remove(SelectedScene);
        SelectedScene = Project.Scenes[Math.Min(index, Project.Scenes.Count - 1)];
        Notice = $"Scene '{removedName}' deleted. Save the project to keep this change.";
    }

    private void AddSource(string type)
    {
        if (SelectedScene is null) return;
        var source = new Source { Type = type, Name = GetUniqueSourceName(type) };
        SelectedScene.Sources.Add(source);
        OnPropertyChanged(nameof(SourceCount));
        Notice = $"{type} source added to '{SelectedScene.Name}'.";
    }

    private string GetUniqueSceneName(string baseName) => GetUniqueName(baseName, Project.Scenes.Select(scene => scene.Name));
    private string GetUniqueSourceName(string baseName) => GetUniqueName(baseName, SelectedScene?.Sources.Select(source => source.Name) ?? []);
    private static string GetUniqueName(string baseName, IEnumerable<string> existingNames)
    {
        var existing = existingNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!existing.Contains(baseName)) return baseName;
        for (var index = 2; ; index++)
        {
            var candidate = $"{baseName} {index}";
            if (!existing.Contains(candidate)) return candidate;
        }
    }

    private async Task SaveProjectAsync()
    {
        try
        {
            var path = await projectService.SaveAsync(Project, projectPath);
            if (path is null)
            {
                Notice = "Project save cancelled.";
                return;
            }

            projectPath = path;
            OnPropertyChanged(nameof(SaveStatus));
            Notice = $"Project saved to {Path.GetFileName(path)}.";
        }
        catch (IOException exception)
        {
            Notice = $"Project could not be saved: {exception.Message}";
        }
        catch (UnauthorizedAccessException)
        {
            Notice = "Project could not be saved because Windows denied access to that location.";
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> action;

    public RelayCommand(Action action) => this.action = _ => action();
    public RelayCommand(Action<object?> action) => this.action = action;
    event EventHandler? ICommand.CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => action(parameter);
}

public sealed class AsyncRelayCommand(Func<Task> action) : ICommand
{
    event EventHandler? ICommand.CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public async void Execute(object? parameter) => await action();
}