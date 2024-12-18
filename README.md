# Support tool



## How to build the application from source. (Using windows 10 and visual studio,)
Build into 1 single exe with no .dlls

 `dotnet publish -r win-x64 -c Release --self-contained /p:PublishSingleFile=true`

 or 
 
 `dotnet publish -c release -r win10-x64 -o ./publish-win10`

 ## Features
 - 4 Different themes (Green, blue, gold, purple)
 - Dark mode / light mode
 - Secure password copying (Clears clipboard after 15 seconds)
 - Quick access to command prompt commands
 - Application specific panel to store teamviewer credentials for computer counting and quick reflecting 
 - A notepad with autosaving functionality 
   - The notepad will resume any saved files if found in /documents
 - Always on top on all forms 



## Documentation 
 Settings file is stored in `C:\Users\$USERNAME$\AppData\Roaming\CopyPastaSettings\CopypastaSettings.txt`

 Autosaved text document from the notepad is saved to `C:\Users\$USERNAME$\Documents\Support tool auto saved notepad.txt`


## Screenshots 

<img src="https://github.com/user-attachments/assets/88799a1b-c8de-47e6-9be9-06b3dc661bc3" alt="Main window screenshot" width="325" height="312">
<img src="https://github.com/user-attachments/assets/707c27b4-6232-4a1d-9eac-300fd89a85e5" alt="Command buttons tab screenshot" width="325" height="312">
<img src="https://github.com/user-attachments/assets/b493f850-a6dd-4141-92d5-c1dd85ce502f" alt="Teamviewer credentials tab screenshot" width="325" height="312">
<img src="https://github.com/user-attachments/assets/53ceba9c-fe0c-41c3-8b35-7486bb06fed2" alt="Notepad window screenshot" width="717" height="359">
