#define MyAppName      "Forex Strategy Trader"
#define MyAppVersion   "3.2.3.0"
#define MyAppVerText   "v3.2.3"
#define MyOutputPath   SourcePath
#define MyCertPass     ReadIni(SourcePath + "\Install.ini", "Release", "CertificatePassoword")

[Setup]
AppName            = {#MyAppName}
AppVersion         = {#MyAppVersion}
VersionInfoVersion = {#MyAppVersion}
AppVerName         = {#MyAppName} {#MyAppVerText}

ArchitecturesInstallIn64BitMode = x64 ia64
AppPublisher       = Forex Software Ltd.
AppPublisherURL    = http://forexsb.com/
AppCopyright       = Copyright © 2009 - 2013 Miroslav Popov
AppComments        = Automatic, visual strategy trader via MetaTrader.
DefaultDirName     = {pf}\{#MyAppName}
DefaultGroupName   = {#MyAppName}
SourceDir          = ..\Output\Release
LicenseFile        = License.rtf
OutputBaseFilename = ForexStrategyTrader
OutputDir          = {#MyOutputPath}
SignTool           = signtool sign /f "{#MyOutputPath}\ForexSoftwareCertificate.pfx" /t "http://timestamp.digicert.com" /p "{#MyCertPass}" /d $q{#MyAppName}$q $f

DisableProgramGroupPage = true
DisableReadyPage        = true

[Components]
Name: "main";      Description: "Main files.";                                                   Types: full compact custom; Flags: fixed;
Name: "autostart"; Description: "autostart.bat (Disable to prevent replacing of existing one)."; Types: full;                Flags: disablenouninstallwarning;
Name: "mt4files";  Description: "MT4 Expert and Library (Required for Trader).";                 Types: full;                Flags: disablenouninstallwarning;
Name: "vcredist";  Description: "VC++ Redist 2012 (Required for MT4 library).";                  Types: full;                Flags: disablenouninstallwarning;

[InstallDelete]
Type: files; Name: "{app}\User Files\System\fstconfig.xml"
Type: files; Name: "{app}\User Files\System\config.xml"
Type: files; Name: "{app}\User Files\System\fst-update.xml"

[Files]
Source: autostart.bat;                      DestDir: "{app}";                                 Components: autostart; Permissions: users-modify;
Source: Forex Strategy Trader.exe;          DestDir: "{app}";                                 Components: main;      Flags: replacesameversion;
Source: FST_Launcher.exe;                   DestDir: "{app}";                                 Components: main;      Flags: replacesameversion;
Source: FST_Launcher.xml;                   DestDir: "{app}";                                 Components: main;
Source: ReadMe.html;                        DestDir: "{app}";                                 Components: main;
Source: License.rtf;                        DestDir: "{app}";                                 Components: main;
Source: Redistributable\*;                  DestDir: "{app}\Redistributable";                 Components: main;
Source: User Files\Indicators\*;            DestDir: "{app}\User Files\Indicators";           Components: main;
Source: User Files\MetaTrader\*;            DestDir: "{app}\User Files\MetaTrader";           Components: main;      Flags: replacesameversion;
Source: User Files\Strategies\*;            DestDir: "{app}\User Files\Strategies";           Components: main;
Source: User Files\System\Colors\*;         DestDir: "{app}\User Files\System\Colors";        Components: main;
Source: User Files\System\Images\*;         DestDir: "{app}\User Files\System\Images";        Components: main;
Source: User Files\System\Languages\*;      DestDir: "{app}\User Files\System\Languages";     Components: main;
Source: User Files\System\Sounds\*;         DestDir: "{app}\User Files\System\Sounds";        Components: main;
Source: User Files\System\StartingTips\*;   DestDir: "{app}\User Files\System\StartingTips";  Components: main;

[Dirs]
Name: "{app}\User Files";                      Permissions: users-modify;
Name: "{app}\User Files\Logs";                 Permissions: users-modify;
Name: "{app}\User Files\Libraries";            Permissions: users-modify;
Name: "{app}\User Files\Strategies";           Permissions: users-modify;
Name: "{app}\User Files\System";               Permissions: users-modify;
Name: "{app}\User Files\System\Languages";     Permissions: users-modify;

[Icons]
Name: "{commondesktop}\Forex Strategy Trader"; Filename: "{app}\FST_Launcher.exe";   WorkingDir: "{app}";
Name: "{group}\Forex Strategy Trader";         Filename: "{app}\FST_Launcher.exe";   WorkingDir: "{app}";
Name: "{group}\User Files";                    Filename: "{app}\User Files";
Name: "{group}\Uninstall";                     Filename: "{uninstallexe}";

[Run]
Filename: "{app}\Redistributable\vcredist_x86.exe";           WorkingDir: "{app}\Redistributable";       StatusMsg: "Installing vcredist_x86...";                  Components: vcredist; Flags: waituntilterminated skipifdoesntexist; Parameters: "/q:a";
Filename: "{app}\User Files\MetaTrader\Install MT Files.exe"; WorkingDir: "{app}\User Files\MetaTrader"; StatusMsg: "Installing the MetaTrader expert and DLL..."; Components: mt4files; Flags: waituntilterminated
Filename: "{app}\ReadMe.html";      Description: "View the ReadMe file";   Flags: postinstall skipifsilent shellexec;
Filename: "{app}\FST_Launcher.exe"; Description: "Launch the application"; Flags: postinstall skipifsilent nowait;

[code]
var
  UsagePage: TInputOptionWizardPage;

procedure InitializeWizard;
begin
  UsagePage := CreateInputOptionPage(wpSelectProgramGroup,
    'Usage Statistics', 'Help us improve Forex Strategy Trader',
    'We would greatly appreciate if you allow us to collect anonymous usage statistics to help us provide a better quality product. The information collected is not used to identify or contact you. You can interrupt collecting usage statistics at any time from Help - Send anonymous usage statistics menu option in the program.',
    True, False);
  UsagePage.Add('Allow anonymous usage statistics (recomended).');
  UsagePage.Add('Do not send anonymous usage statistics.');
  UsagePage.SelectedValueIndex := 0;
end;

function InitializeSetup(): Boolean;
var
  ErrorCode : Integer;
  Result1   : Boolean;
begin
	Result := RegKeyExists(HKLM,'SOFTWARE\Microsoft\.NETFramework\policy\v2.0');
	if Result = false then
	  begin
      Result1 := MsgBox('This setup requires the .NET Framework v2.0. Please download and install the .NET Framework v.2 and run this setup again. Do you want to download the framework now?', mbConfirmation, MB_YESNO) = idYes;
		  if Result1 = true then
		    begin
			    ShellExec('open', 'http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe', '','', SW_SHOWNORMAL, ewNoWait, ErrorCode);
		    end;
    end;
 end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  if CurPageID = UsagePage.ID then
  begin
    RegWriteStringValue(HKCU, 'Software\Forex Software\Forex Strategy Trader', 'UsageStats', IntToStr(UsagePage.SelectedValueIndex));
  end;
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssInstall then
  begin
    // Delete legacy content
    DeleteFile(ExpandConstant('{app}\FST Starter.exe'));
    DeleteFile(ExpandConstant('{app}\SplashConfig.cfg'));
    DeleteFile(ExpandConstant('{app}\ConnectWait.ico'));
    DelTree(ExpandConstant('{app}\SplashScreen'), True, True, True);
    DelTree(ExpandConstant('{app}\User Files\System\SplashScreen'), True, True, True);

    // Move files in new location
    CreateDir(ExpandConstant('{app}\User Files'));
    RenameFile(ExpandConstant('{app}\Logs'),              ExpandConstant('{app}\User Files\Logs'));
    RenameFile(ExpandConstant('{app}\Strategies'),        ExpandConstant('{app}\User Files\Strategies'));
    RenameFile(ExpandConstant('{app}\System'),            ExpandConstant('{app}\User Files\System'));
    DelTree(ExpandConstant('{app}\Custom Indicators'), True, True, True);
    DelTree(ExpandConstant('{app}\Logs'),              True, True, True);
    DelTree(ExpandConstant('{app}\MetaTrader'),        True, True, True);
    DelTree(ExpandConstant('{app}\Strategies'),        True, True, True);
    DelTree(ExpandConstant('{app}\System'),            True, True, True);
  end;
end;
