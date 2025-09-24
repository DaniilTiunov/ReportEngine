; Inno Setup Script for Смета КИП
#define MyAppName "Смета КИП"
#define MyAppVersion "0.0.0.1"
#define MyAppPublisher "ETALON"
#define MyAppURL "https://www.example.com/"
#define MyAppExeName "ReportEngine.App.exe"
#define MyAppAssocName MyAppName + " File"
#define MyAppAssocExt ".myp"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
AppId={{A6BEA5AB-0140-4F05-AB93-338C2B2EE796}-{#MyAppVersion}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}\{#MyAppVersion}
UninstallDisplayIcon={app}\{#MyAppExeName},0
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
ChangesAssociations=yes
DisableProgramGroupPage=yes
PrivilegesRequiredOverridesAllowed=dialog
OutputBaseFilename=setup
SetupIconFile=C:\Work\Prjs\ReportEngine\ReportEngine.App\Resources\Icons\install_line_icon_236048.ico
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "dotnet"; Description: "Установить .NET 8.0?"; GroupDescription: "Дополнительные компоненты"; Flags: unchecked

[Files]
; Копируем все файлы из папки Release, включая Updater.exe
Source: "C:\Work\Share\Output\ReportEngineRelease\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; .NET Runtime для первой установки
Source: "C:\Work\Установщики\dotnet-sdk-8.0.317-win-x64.exe"; DestDir: "{tmp}"; Flags: ignoreversion

[Registry]
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\estimate_cost_icon_213382_prev.ico"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\estimate_cost_icon_213382_prev.ico"; Tasks: desktopicon

[Run]
; Устанавливаем .NET Runtime, если выбрана галочка "dotnet"
Filename: "{tmp}\dotnet-sdk-8.0.317-win-x64.exe"; Parameters: "/quiet /norestart"; StatusMsg: "Установка .NET Desktop Runtime 8.0..."; Tasks: dotnet

; Запуск основного приложения после установки
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
