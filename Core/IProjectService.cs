namespace LubbInteractiveCreator.Core;

public interface IProjectService
{
    Task<string?> SaveAsync(Project project, string? path = null);
    Task<Project?> OpenAsync(string path);
}