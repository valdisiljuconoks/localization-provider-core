cd .\.nuget

.\nuget.exe push LocalizationProvider.AspNetCore.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.Csv.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.Xliff.8.0.0.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.8.0.0.nupkg -source https://api.nuget.org/v3/index.json

cd ..\
