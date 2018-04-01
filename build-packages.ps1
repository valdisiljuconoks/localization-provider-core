cd .\.nuget

cd .\..\src\DbLocalizationProvider.AspNetCore\
dotnet pack -c Release
copy .\bin\Release\LocalizationProvider.AspNetCore.*.nupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.AdminUI.AspNetCore\
dotnet pack -c Release
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.*.nupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd ..\
