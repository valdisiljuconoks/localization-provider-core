cd .\.nuget

cd .\..\src\DbLocalizationProvider.AspNetCore\
dotnet pack -c Release
copy .\bin\Release\*.nupkg .\..\..\.nuget\
cd .\..\..\.nuget\

cd ..\
