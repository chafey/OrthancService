Orthanc On Windows 
===================

This project contains the following windows specific extensions for Orthanc:

OrthancSerivce - A C# Windows Service that will launch the Orthanc server (and restart it if it crashes)
Installer - A NSIS installer for Orthanc
ConfigurationFileWriter - A C# console application that will patch the configuration files during installation based on the selected folders

Building
========

Pre-requisites
--------------

1. Visual Studio 2013 Professional (may work on earlier versions but has not been tested)
2. NSIS 2.46 (may work with other versions but has not been tested)

Building
--------

1. Open the solution, make sure "Release" build configuration is selected
2. Copy the build of Orthanc.exe you want to build into the installer into the "InstallerAssets" directory.  
3. Build the solution (F6)
4. Using windows explorer, right click on the Installer/OrthancInstaller.nsi and select "Compile NSIS Script".  

The installer will be located in OrthancInstaller/OrthancInstaller.exe

FAQ
==============

_Orthanc Service is running but I cannot connect to Orthan or send it DICOM_

Check the Orthanc log in $(DATADIRECTORY)/Logs to see if that helps
