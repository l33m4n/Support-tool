# Support tool



## How to build the application from source. (Using windows 10 and visual studio,)
Build into 1 single exe with no .dlls

 dotnet publish -r win-x64 -c Release --self-contained /p:PublishSingleFile=true

 or 
 
  dotnet publish -c release -r win10-x64 -o ./publish-win10



## Documentation 
 Settings file is stored in C:\Users\$USERNAME$\AppData\Roaming\CopyPastaSettings\CopypastaSettings.txt
