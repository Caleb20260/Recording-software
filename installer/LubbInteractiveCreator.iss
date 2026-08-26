#define AppName "Lubb Interactive Creator"
#define AppVersion "0.3.1"
#define Publisher "Lubb Interactive"
#define AppExeName "LubbInteractiveCreator.exe"

[Setup]
AppId={{BDA9D98D-1BBE-4E82-BB3B-202600000001}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#Publisher}
AppPublisherURL=https://lubbinteractive.example
AppCopyright=(C) 2026 Lubb Interactive. All Rights Reserved.
DefaultDirName={autopf}\Lubb Interactive Creator
DefaultGroupName={#AppName}
OutputBaseFilename=LubbInteractiveCreatorSetup-{#AppVersion}
OutputDir=..\artifacts\installer
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\{#AppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
Source: "..\artifacts\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent