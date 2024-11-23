# Support tool



## How to build the application from source. (Using windows 10 and visual studio,)
Build into 1 single exe with no .dlls

 dotnet publish -r win-x64 -c Release --self-contained /p:PublishSingleFile=true

 or 
 
  dotnet publish -c release -r win10-x64 -o ./publish-win10



## Documentation 
 Settings file is stored in C:\Users\$USERNAME$\AppData\Roaming\CopyPastaSettings\CopypastaSettings.txt


## Resized screenshots 
main window screenshot

<img src="https://github.com/user-attachments/assets/88799a1b-c8de-47e6-9be9-06b3dc661bc3" alt="Main window screenshot" width="325" height="312">
<img src="https://github.com/user-attachments/assets/707c27b4-6232-4a1d-9eac-300fd89a85e5" alt="Command buttons tab screenshot" width="325" height="312">
<img src="https://github.com/user-attachments/assets/b493f850-a6dd-4141-92d5-c1dd85ce502f" alt="Teamviewer credentials tab screenshot" width="325" height="312">
<img src="https://github.com/user-attachments/assets/53ceba9c-fe0c-41c3-8b35-7486bb06fed2" alt="Notepad window screenshot" width="325" height="162">
