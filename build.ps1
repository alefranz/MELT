dotnet format --verify-no-changes MELT.sln
dotnet build --configuration Release MELT.sln
dotnet test --no-build --configuration Release MELT.sln
