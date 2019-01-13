cd .\.nuget

cd .\..\src\DbLocalizationProvider.AspNetCore\
dotnet pack -c Release
copy .\bin\Release\LocalizationProvider.AspNetCore.*.nupkg .\..\..\.nuget\
copy .\bin\Release\LocalizationProvider.AspNetCore.*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd .\..\src\DbLocalizationProvider.AdminUI.AspNetCore\
dotnet pack -c Release
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.*.nupkg .\..\..\.nuget\
copy .\bin\Release\LocalizationProvider.AdminUI.AspNetCore.*.snupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd ..\
