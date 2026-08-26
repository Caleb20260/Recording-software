using System.ComponentModel;
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

    public ICommand NewProjectCommand { get; }
    public ICommand SaveProjectCommand { get; }
    public ICommand StartRecordingCommand { get; }
    public ICommand GoLiveCommand { get; }
    public ICommand ConnectXboxCommand { get; }
    public ICommand ConnectDiscordCommand { get; }
    public ICommand SaveReplayCommand { get; }
    public ICommand CreateClipCommand { get; }
    public ICommand PrivacyCommand { get; }

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
    }

    private void NewProject()
    {
        Project = new Project();
        projectPath = null;
        OnPropertyChanged(nameof(SaveStatus));
        Notice = "New project created. Save it to begin building your workspace.";
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

public sealed class RelayCommand(Action action) : ICommand
{
    event EventHandler? ICommand.CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => action();
}

public sealed class AsyncRelayCommand(Func<Task> action) : ICommand
{
    event EventHandler? ICommand.CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public async void Execute(object? parameter) => await action();
}