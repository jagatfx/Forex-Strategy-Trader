#define MyAppName      "Forex Strategy Trader"
#define MyAppVersion   "3.0.0.0"
#define MyAppVerText   "v3"
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
Name: "main";      Description: "Main files.";        Types: full compact custom; Flags: fixed;
Name: "autostart"; Description: "autostart.bat (Disable to prevent replacing of existing one)."; Types: full; Flags: disablenouninstallwarning;
Name: "custind";   Description: "Custom indicators."; Types: full; Flags: disablenouninstallwarning;
Name: "strat";     Description: "Demo strategies.";   Types: full; Flags: disablenouninstallwarning;
Name: "vcredist";  Description: "VC Redist 2008 (Required. If you are not sure, leave it on)."; Types: full; Flags: disablenouninstallwarning;

[InstallDelete]
Type: files; Name: "{app}\User Files\System\fstconfig.xml"
Type: files; Name: "{app}\User Files\System\config.xml"
Type: files; Name: "{app}\User Files\System\fst-update.xml"

[Files]
Source: autostart.bat;                      DestDir: "{app}"; Components: autostart;   Permissions: users-modify;
Source: Forex Strategy Trader.exe;          DestDir: "{app}"; Components: main;        Flags: replacesameversion;
Source: FST_Launcher.exe;                   DestDir: "{app}"; Components: main;        Flags: replacesameversion;
Source: FST_Launcher.xml;                   DestDir: "{app}"; Components: main;
Source: ReadMe.html;                        DestDir: "{app}"; Components: main;
Source: License.rtf;                        DestDir: "{app}"; Components: main;
Source: Redistributable\*;                  DestDir: "{app}\Redistributable";                 Components: vcredist;
Source: User Files\Indicators\*;            DestDir: "{app}\User Files\Indicators";           Components: custind;
Source: User Files\MetaTrader\*;            DestDir: "{app}\User Files\MetaTrader";           Components: main;
Source: User Files\Strategies\*;            DestDir: "{app}\User Files\Strategies";           Components: strat;
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
Filename: "{app}\Redistributable\vcredist_x86.exe"; WorkingDir: "{app}\Redistributable"; Parameters: "/q:a"; StatusMsg: "Installing vcredist_x86..."; Flags: waituntilterminated skipifdoesntexist
Filename: "{app}\User Files\MetaTrader\Install MT Files.exe"; WorkingDir: "{app}\User Files\MetaTrader"; StatusMsg: "Installing the MetaTrader expert and DLL...";    Flags: waituntilterminated
Filename: "{app}\ReadMe.html";     Description: "View the ReadMe file";   Flags: postinstall skipifsilent shellexec;
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
    
   MsgBox('Meta Trader 4 have to be closed during the installation.', mbConfirmation, MB_OK);
end;

procedure DeleteVCRedistRuntimeTemporaryFiles();
var
   i : Integer;
   byCounter : Byte;
   strRootDrivePath : String;
   arrFiles : Array [1..24] Of String;
begin
   For byCounter := 67 to 77 do
   Begin
      strRootDrivePath := Chr(byCounter) + ':\';
      arrFiles[1] := strRootDrivePath + 'vcredist.bmp';
      arrFiles[2] := strRootDrivePath + 'VC_RED.cab';
      arrFiles[3] := strRootDrivePath + 'VC_RED.MSI';

      If (FileExists(arrFiles[1]) And FileExists(arrFiles[2]) And FileExists(arrFiles[3])) Then
      Begin
          arrFiles[4]  := strRootDrivePath + 'eula.1028.txt';
          arrFiles[5]  := strRootDrivePath + 'eula.1031.txt';
          arrFiles[6]  := strRootDrivePath + 'eula.1033.txt';
          arrFiles[7]  := strRootDrivePath + 'eula.1036.txt';
          arrFiles[8]  := strRootDrivePath + 'eula.1040.txt';
          arrFiles[9]  := strRootDrivePath + 'eula.1041.txt';
          arrFiles[10] := strRootDrivePath + 'eula.1042.txt';
          arrFiles[11] := strRootDrivePath + 'eula.2052.txt';
          arrFiles[12] := strRootDrivePath + 'eula.3082.txt';
          arrFiles[13] := strRootDrivePath + 'globdata.ini';
          arrFiles[14] := strRootDrivePath + 'install.exe';
          arrFiles[15] := strRootDrivePath + 'install.ini';
          arrFiles[16] := strRootDrivePath + 'install.res.1028.dll';
          arrFiles[17] := strRootDrivePath + 'install.res.1031.dll';
          arrFiles[18] := strRootDrivePath + 'install.res.1033.dll';
          arrFiles[19] := strRootDrivePath + 'install.res.1036.dll';
          arrFiles[20] := strRootDrivePath + 'install.res.1040.dll';
          arrFiles[21] := strRootDrivePath + 'install.res.1041.dll';
          arrFiles[22] := strRootDrivePath + 'install.res.1042.dll';
          arrFiles[23] := strRootDrivePath + 'install.res.2052.dll';
          arrFiles[24] := strRootDrivePath + 'install.res.3082.dll';

          For i := 1 to 24 Do
          Begin
            DeleteFile(arrFiles[i]);
          End;

          Break;
      End;
   End;
End;

procedure DeinitializeSetup();
begin
    DeleteVCRedistRuntimeTemporaryFiles();
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
    RenameFile(ExpandConstant('{app}\Custom Indicators'), ExpandConstant('{app}\User Files\Indicators'));
    RenameFile(ExpandConstant('{app}\Logs'),              ExpandConstant('{app}\User Files\Logs'));
    RenameFile(ExpandConstant('{app}\MetaTrader'),        ExpandConstant('{app}\User Files\MetaTrader'));
    RenameFile(ExpandConstant('{app}\Strategies'),        ExpandConstant('{app}\User Files\Strategies'));
    RenameFile(ExpandConstant('{app}\System'),            ExpandConstant('{app}\User Files\System'));
    DelTree(ExpandConstant('{app}\Custom Indicators'), True, True, True);
    DelTree(ExpandConstant('{app}\Logs'),              True, True, True);
    DelTree(ExpandConstant('{app}\MetaTrader'),        True, True, True);
    DelTree(ExpandConstant('{app}\Strategies'),        True, True, True);
    DelTree(ExpandConstant('{app}\System'),            True, True, True);
  end;
end;
