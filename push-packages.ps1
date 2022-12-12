cd .\.nuget

.\nuget.exe push LocalizationProvider.AspNetCore.7.5.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.Csv.7.5.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.Xliff.7.5.1.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.7.5.1.nupkg -source https://api.nuget.org/v3/index.json

cd ..\
