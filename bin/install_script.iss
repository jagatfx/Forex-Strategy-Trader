[Setup]
AppName            = Forex Strategy Trader
AppVersion         = 1.5.1.0
VersionInfoVersion = 1.5.1.0
AppVerName         = Forex Strategy Trader v1.5.1.0 NB

ArchitecturesInstallIn64BitMode = x64 ia64
AppPublisher       = Forex Software Ltd.
AppPublisherURL    = http://forexsb.com/
AppCopyright       = Copyright (C) 2012 Miroslav Popov
AppComments        = Automatic, visual strategy trader via MetaTrader.
DefaultDirName     = {pf}\Forex Strategy Trader
DefaultGroupName   = Forex Strategy Trader
SourceDir          = Forex Strategy Trader
LicenseFile        = License.rtf
OutputBaseFilename = ForexStrategyTrader
OutputDir          = ..\

DisableProgramGroupPage = true
DisableReadyPage        = true

[Components]
Name: "main";      Description: "Main files.";        Types: full compact custom; Flags: fixed;
Name: "autostart"; Description: "autostart.bat (Disable to prevent overwriting of existing one)."; Types: full; Flags: disablenouninstallwarning;
Name: "custind";   Description: "Custom indicators."; Types: full; Flags: disablenouninstallwarning;
Name: "strat";     Description: "Demo strategies.";   Types: full; Flags: disablenouninstallwarning;
Name: "vcredist";  Description: "VC Redist 2008 (Required. Leave on if not previously installed)."; Types: full; Flags: disablenouninstallwarning;

[InstallDelete]
Type: files; Name: "{app}\System\fstconfig.xml"
Type: files; Name: "{app}\System\config.xml"
Type: files; Name: "{app}\System\fst-update.xml"
Type: files; Name: "{app}\Redistributable\vcredist_x86.exe"

[Files]
Source: autostart.bat;              DestDir: "{app}"; Components: autostart;   Permissions: users-modify;
Source: Forex Strategy Trader.exe;  DestDir: "{app}"; Components: main;        Flags: replacesameversion;
Source: FST Starter.exe;            DestDir: "{app}"; Components: main;
Source: SplashConfig.cfg;           DestDir: "{app}"; Components: main;
Source: ConnectWait.ico;            DestDir: "{app}"; Components: main;
Source: ReadMe.html;                DestDir: "{app}"; Components: main;
Source: License.rtf;                DestDir: "{app}"; Components: main;
Source: Custom Indicators\*;        DestDir: "{app}\Custom Indicators";    Components: custind;
Source: MetaTrader\*;               DestDir: "{app}\MetaTrader";           Components: main;
Source: Redistributable\*;          DestDir: "{app}\Redistributable";      Components: vcredist;
Source: Strategies\*;               DestDir: "{app}\Strategies";           Components: strat;
Source: System\Colors\*;            DestDir: "{app}\System\Colors";        Components: main;
Source: System\Images\*;            DestDir: "{app}\System\Images";        Components: main;
Source: System\Languages\*;         DestDir: "{app}\System\Languages";     Components: main;
Source: System\Sounds\*;            DestDir: "{app}\System\Sounds";        Components: main;
Source: System\SplashScreen\*;      DestDir: "{app}\System\SplashScreen";  Components: main;
Source: System\StartingTips\*;      DestDir: "{app}\System\StartingTips";  Components: main;

[Dirs]
Name: "{app}\Custom Indicators";    Permissions: users-modify;
Name: "{app}\Strategies";           Permissions: users-modify;
Name: "{app}\System";               Permissions: users-modify;
Name: "{app}\System\SplashScreen";  Permissions: users-modify;
Name: "{app}\System\Languages";     Permissions: users-modify;

[Icons]
Name: "{commondesktop}\Forex Strategy Trader"; Filename: "{app}\FST Starter.exe";   WorkingDir: "{app}";
Name: "{group}\Forex Strategy Trader";         Filename: "{app}\FST Starter.exe";   WorkingDir: "{app}";
Name: "{group}\Custom Indicators";             Filename: "{app}\Custom Indicators";
Name: "{group}\MetaTrader";                    Filename: "{app}\MetaTrader";
Name: "{group}\Uninstall";                     Filename: "{uninstallexe}";

[Run]
Filename: "{app}\Redistributable\vcredist_x86.exe"; WorkingDir: "{app}\Redistributable"; Parameters: "/q:a"; StatusMsg: "Installing vcredist_x86..."; Flags: waituntilterminated skipifdoesntexist
Filename: "{app}\MetaTrader\Install MT Files.exe"; WorkingDir: "{app}\MetaTrader"; StatusMsg: "Installing the MetaTrader expert and DLL...";    Flags: waituntilterminated
Filename: "{app}\ReadMe.html";     Description: "View the ReadMe file";   Flags: postinstall skipifsilent shellexec;
Filename: "{app}\FST Starter.exe"; Description: "Launch the application"; Flags: postinstall skipifsilent nowait;

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
