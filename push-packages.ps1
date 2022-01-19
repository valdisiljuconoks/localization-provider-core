cd .\.nuget

.\nuget.exe push LocalizationProvider.AspNetCore.7.0.0-pre-0015.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.Csv.7.0.0-pre-0015.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.Xliff.7.0.0-pre-0015.nupkg -source https://api.nuget.org/v3/index.json
.\nuget.exe push LocalizationProvider.AdminUI.AspNetCore.7.0.0-pre-0015.nupkg -source https://api.nuget.org/v3/index.json

cd ..\
