echo "This command must be run in an Administrator shell."
sc create "Orthanc Service" binpath= "c:\Orthanc\OrthancService.exe" type= "own" start= "auto" 
sc description "Orthanc Service" "Manages the Orthanc Server as Service."