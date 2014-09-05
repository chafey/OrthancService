Orthanc For Windows
===================

By default, Orthanc listens to incoming DICOM connections on port 4242 and incoming HTTP connections
on port 8042.  You can launch the Orthanc Explorer by clicking the link in the start menu or opening
your web browser to http://127.0.0.1:8042/app/explorer.html.  

The Orthanc configuration file can be found in the data folder with the name Configuration.json.
The Orthanc Installer always overwrites the Orthanc configuration so you should make a backup 
copy before if you make any changes to it before reinstalling Orthanc.

The start menu includes a shortcut to an uninstaller.  Running the uninstaller will remove the Orthanc
program files form your system, but it does not delete any data saved in Orthanc (e.g. DICOM files).
The data is left behind to make it easy to upgrade to a newer version of Orthanc without losing
the data it has saved.  

If you wish to delete the data Orthanc has saved, delete the data folder (e.g. C:\Orthanc) after 
uninstalling Orthanc.  Keep this in mind if you use Orthanc to store protected health information (PHI).

Troubleshooting
---------------

1. Verify that the Orthanc Service is running using the Services control panel.   
2. Verify that the firewall is configured to allow inbound connections to the ports Orthanc is listening on
3. Verify that no other applications are using the ports that Orthanc is listening on
