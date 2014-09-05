Orthanc For Windows
===================

Orthanc For Windows includes windows specific extensions for Orthanc such as an installer and
a windows service.  The installer and windows service both require the .Net 4.0 client framework
already be installed on the machine.  You can learn more about Orthanc by 
visiting http://www.orthanc-server.com/

The installer will prompt you for two folders - one to use for program files and one to
use for data.  The data folder is used for orthanc storage, orthanc database, orthanc logs 
and configuration files.  

By default, Orthanc listens to incoming DICOM connections on port 4242 and incoming HTTP connections
on port 8042.  You can launch the Orthanc Explorer by clicking the link in the start menu or opening
your web browser to http://127.0.0.1:8042/app/explorer.html.  

The start menu includes a shortcut to an uninstaller.  Running the uninstaller will remove the Orthanc
program files form your system, but it does not delete any data folder.  The data folder is left behind 
to make it easy to upgrade to a newer version of Orthanc without losing the data it has saved.  

If you wish to delete the data Orthanc has saved, delete the data folder (e.g. C:\Orthanc) after 
uninstalling Orthanc.  Keep this in mind if you use Orthanc to store protected health information (PHI).

The Orthanc configuration file can be found in the data folder with the name Configuration.json.
The Orthanc Installer always overwrites the Orthanc configuration so you should make a backup 
copy if you make any changes to it before reinstalling Orthanc.

Troubleshooting
---------------

1. Verify that the Orthanc Service is running using the Services control panel.   
2. Verify that the firewall is configured to allow inbound connections to the ports Orthanc is listening on
3. Verify that no other applications are using the ports that Orthanc is listening on
4. Check the Orthanc logs in the data folder (e.g. C:\Orthanc\Logs)
5. Check the Orthanc Service log in %APPDATA%\Orthanc\OrthancService.log
6. Check the Orthanc FAQ: https://code.google.com/p/orthanc/wiki/FAQ
7. Post a request for help to the Orthanc Users google group: https://groups.google.com/forum/#!forum/orthanc-users 
