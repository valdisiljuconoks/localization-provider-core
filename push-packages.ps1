cd .\.nuget

nuget push .\LocalizationProvider.AspNetCore.5.3.0.nupkg -source https://api.nuget.org/v3/index.json
nuget push .\LocalizationProvider.AdminUI.AspNetCore.5.3.0.nupkg -source https://api.nuget.org/v3/index.json

cd ..\
