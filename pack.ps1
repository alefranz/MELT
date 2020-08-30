param (
    [string]$Version
)

if (!$Version){
    $Version = $(git describe --tags --dirty)
    if (!$Version) {
        Write-Error "Create valid tag or use -Version"
        Exit 1
    }
}

Write-Host "Version: $Version`n"

$artifactsPath = Join-Path $PSScriptRoot artifacts

if (Test-Path -Path $artifactsPath) {
    Remove-Item $artifactsPath -Recurse
}
New-Item -Path $artifactsPath -ItemType directory > $null

dotnet pack -c Release -o $artifactsPath -p:Version=$Version -p:GITHUB_ACTIONS=true
