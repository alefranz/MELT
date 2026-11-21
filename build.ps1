$ErrorActionPreference = 'Stop'

dotnet format --verify-no-changes MELT.sln
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet build --configuration Release MELT.sln
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet test --no-build --configuration Release MELT.sln
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
