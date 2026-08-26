namespace LubbInteractiveCreator.Core;

public sealed record SecurityState(
    string ApplicationIntegrity,
    string PluginSecurity,
    string CredentialStorage,
    string UpdateSecurity,
    bool AnalyticsEnabled,
    bool CrashReportsEnabled);