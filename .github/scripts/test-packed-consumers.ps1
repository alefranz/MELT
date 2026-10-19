[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$PackageDirectory
)

$ErrorActionPreference = 'Stop'
$packageDirectory = (Resolve-Path $PackageDirectory).Path
$meltPackage = Get-ChildItem -Path $packageDirectory -Filter 'MELT.*.nupkg' |
    Where-Object { $_.Name -notlike '*.symbols.nupkg' } |
    Select-Object -First 1

if ($null -eq $meltPackage -or $meltPackage.Name -notmatch '^MELT\.(?<Version>.+)\.nupkg$') {
    throw "Could not determine the MELT package version from '$packageDirectory'."
}

$packageVersion = $Matches.Version
$consumersRoot = Join-Path $PSScriptRoot '../../test/PackageConsumers'
$consumers = @(
    @{ Name = 'Consumer.Net8'; Framework = 'net8.0'; Packages = @{
            'MELT' = 'lib/netstandard2.0/MELT.dll'
            'MELT.Serilog' = 'lib/netstandard2.0/MELT.Serilog.dll'
            'MELT.Xunit' = 'lib/netstandard2.0/MELT.Xunit.dll'
            'MELT.Xunit.v3' = 'lib/netstandard2.0/MELT.Xunit.v3.dll'
            'MELT.AspNetCore' = 'lib/net8.0/MELT.AspNetCore.dll'
            'MELT.Serilog.AspNetCore' = 'lib/net8.0/MELT.Serilog.AspNetCore.dll'
        } }
    @{ Name = 'Consumer.Net9'; Framework = 'net9.0'; Packages = @{
            'MELT' = 'lib/netstandard2.0/MELT.dll'
            'MELT.Serilog' = 'lib/netstandard2.0/MELT.Serilog.dll'
            'MELT.Xunit' = 'lib/netstandard2.0/MELT.Xunit.dll'
            'MELT.Xunit.v3' = 'lib/netstandard2.0/MELT.Xunit.v3.dll'
        } }
    @{ Name = 'Consumer.Net10'; Framework = 'net10.0'; Packages = @{
            'MELT' = 'lib/netstandard2.0/MELT.dll'
            'MELT.Serilog' = 'lib/netstandard2.0/MELT.Serilog.dll'
            'MELT.Xunit' = 'lib/netstandard2.0/MELT.Xunit.dll'
            'MELT.Xunit.v3' = 'lib/netstandard2.0/MELT.Xunit.v3.dll'
            'MELT.AspNetCore' = 'lib/net10.0/MELT.AspNetCore.dll'
            'MELT.Serilog.AspNetCore' = 'lib/net10.0/MELT.Serilog.AspNetCore.dll'
        } }
)

foreach ($consumer in $consumers) {
    $project = Join-Path $consumersRoot "$($consumer.Name)/$($consumer.Name).csproj"
    dotnet run --project $project --property:MeltPackageVersion=$packageVersion --property:MeltPackageSource=$packageDirectory
    if ($LASTEXITCODE -ne 0) { throw "$($consumer.Name) did not run successfully." }

    $assetsPath = Join-Path (Split-Path $project) 'obj/project.assets.json'
    $assets = Get-Content -Raw $assetsPath | ConvertFrom-Json
    $target = $assets.targets.PSObject.Properties[$consumer.Framework].Value
    if ($null -eq $target) { throw "$($consumer.Name) did not restore a $($consumer.Framework) target." }

    foreach ($package in $consumer.Packages.GetEnumerator()) {
        $packageAssets = $target.PSObject.Properties["$($package.Key)/$packageVersion"].Value
        if ($null -eq $packageAssets) { throw "$($consumer.Name) did not restore $($package.Key) $packageVersion." }

        $compileAssets = @($packageAssets.compile.PSObject.Properties.Name)
        if ($compileAssets -notcontains $package.Value) {
            throw "$($consumer.Name) selected an unexpected asset for $($package.Key): $($compileAssets -join ', ')"
        }
    }
}
