mkdir blazor-electron-demo
cd blazor-electron-demo
dotnet new blazorserver --no-https
dotnet add package ElectronNET.API
dotnet new tool-manifest
dotnet tool install ElectronNET.CLI
dotnet electronize init