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

Write-Host $Version

New-Item -Path ./artifacts -Force -ItemType directory

$artifacts = (Get-Item ./artifacts).FullName

dotnet pack -c Release -o $artifacts -p:Version=$Version
