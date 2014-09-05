!include "MUI2.nsh"

!define APPNAME "Orthanc"
!define DESCRIPTION "RESTful DICOM Server"
!define SERVICENAME "Orthanc Service"
!define dataDir "C:\Orthanc"

Var DataDir

; The name of the installer
Name "Orthanc"

; The file to write
OutFile "OrthancInstaller.exe"

; The default installation directory
InstallDir "$PROGRAMFILES32\${APPNAME}"

!define MUI_ABORTWARNING

# Show the installation details while debugging (comment out for release)
ShowInstDetails show
!define MUI_FINISHPAGE_NOAUTOCLOSE

;Pages
	
!insertmacro MUI_PAGE_WELCOME
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE InstDirPageLeave
!insertmacro MUI_PAGE_DIRECTORY
!define MUI_DIRECTORYPAGE_VARIABLE $DataDir
!define MUI_DIRECTORYPAGE_TEXT_TOP "Setup will configure Orthanc to store its data in the following folder.  To use a different folder, click Browse and select another folder.  Click next to continue."
!define MUI_DIRECTORYPAGE_TEXT_DESTINATION "Data Directory:"
!define MUI_PAGE_CUSTOMFUNCTION_SHOW DataDirShowPage
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_FUNCTION "InstallationComplete"
!define MUI_FINISHPAGE_RUN_TEXT "Launch Orthonc Explorer"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
!insertmacro MUI_LANGUAGE "English"
 
Function InstallationComplete
	# Launch the orthonc explorer
	ExecShell "open" "http://127.0.0.1:8042/app/explorer.html"
FunctionEnd

Function InstDirPageLeave
StrCpy $DataDir "C:\Orthanc"
FunctionEnd

Function DataDirShowPage
!insertmacro MUI_HEADER_TEXT "Choose Data Location" "Choose the folder in which Orthanc will store its data and configuration"
FunctionEnd

# default section start; every NSIS script has at least one section.
Section

	# Install program files
	SetOutPath $INSTDIR
	File "..\InstallerAssets\Orthanc.exe"
	File "..\InstallerAssets\README.txt"
	File "..\OrthancService\bin\Release\OrthancService.exe"
	File "..\OrthancService\bin\Release\NLog.dll"
	File "..\OrthancService\bin\Release\OrthancService.exe.config"
	File "..\ConfigurationFileWriter\bin\Release\ConfigurationFileWriter.exe"

	# Install configuration file
	SetOutPath "$DataDir"
	File "Configuration.json"
	File "OrthancServiceConfiguration.json"

	# Patch the configuration files
	nsExec::Exec '$INSTDIR\ConfigurationFileWriter.exe "$DataDir" "$INSTDIR"'

	# Create the uninstaller
	WriteUninstaller "$INSTDIR\uninstall.exe"

	# Add Start Menu Items
	SetShellVarContext all
	CreateDirectory "$SMPROGRAMS\${APPNAME}"
	WriteINIStr "$SMPROGRAMS\${APPNAME}\Orthanc Explorer.url" "InternetShortcut" "URL" "http://127.0.0.1:8042/app/explorer.html"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\uninstall.lnk" "$INSTDIR\uninstall.exe" "" 
	CreateShortCut "$SMPROGRAMS\${APPNAME}\README.lnk" "$INSTDIR\README.txt" "" 
	
	# install the OrthancService and start it
	nsExec::Exec 'sc create "${ServiceName}" binpath= "$INSTDIR\OrthancService.exe C:\Orthanc\OrthancServiceConfiguration.json" type= "own" start= "auto"'
	nsExec::Exec 'sc description "${ServiceName}" "RESTful DICOM Server"'
	nsExec::Exec 'sc start "${ServiceName}"'

# default section end
SectionEnd

# uninstaller section start
Section "uninstall"

	# stop and delete the service
	nsExec::Exec 'sc stop "${ServiceName}"'
	nsExec::Exec 'sc delete "${ServiceName}"'
 
 	# remove start menu items and folder
	SetShellVarContext all
    RMDir /r "$SMPROGRAMS\${APPNAME}"

	#remove the installation directory
	RMDir /r "$INSTDIR"

# uninstaller section end
SectionEnd
