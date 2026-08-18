[CmdletBinding()]
param(
    [string]$Solution = 'MELT.sln',

    [Parameter(Mandatory = $true)]
    [string]$ReportPath
)

$ErrorActionPreference = 'Stop'

function Get-VulnerablePackage {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Node
    )

    if ($Node -is [System.Collections.IEnumerable] -and $Node -isnot [string]) {
        foreach ($item in $Node) {
            Get-VulnerablePackage -Node $item
        }

        return
    }

    if ($Node -isnot [pscustomobject]) {
        return
    }

    $id = $Node.PSObject.Properties['id']
    $vulnerabilities = $Node.PSObject.Properties['vulnerabilities']
    if ($null -ne $id -and $null -ne $vulnerabilities -and @($Node.vulnerabilities).Count -gt 0) {
        [pscustomobject]@{
            Id = $Node.id
            ResolvedVersion = $Node.resolvedVersion
            Vulnerabilities = @($Node.vulnerabilities)
        }
    }

    foreach ($property in $Node.PSObject.Properties) {
        Get-VulnerablePackage -Node $property.Value
    }
}

$reportDirectory = Split-Path -Parent $ReportPath
if ($reportDirectory) {
    New-Item -ItemType Directory -Force -Path $reportDirectory | Out-Null
}

dotnet list $Solution package --vulnerable --include-transitive --format json --output-version 1 --no-restore |
    Tee-Object -FilePath $ReportPath | Out-Host
if ($LASTEXITCODE -ne 0) {
    throw 'The vulnerability scan could not complete.'
}

try {
    $report = Get-Content -Raw $ReportPath | ConvertFrom-Json
}
catch {
    throw "The vulnerability scan did not produce a valid JSON report at '$ReportPath'."
}

$problems = @($report.problems | Where-Object { $_.level -eq 'error' })
if ($problems.Count -gt 0) {
    throw "The vulnerability scan reported errors: $($problems.text -join '; ')"
}

$vulnerablePackages = @(Get-VulnerablePackage -Node $report)
if ($vulnerablePackages.Count -gt 0) {
    $details = $vulnerablePackages | ForEach-Object {
        $advisories = $_.Vulnerabilities | ForEach-Object {
            "$($_.severity): $($_.advisoryurl)"
        }

        "$($_.Id) $($_.ResolvedVersion) ($($advisories -join ', '))"
    }

    $detailText = $details -join [Environment]::NewLine
    throw "Known vulnerable dependencies were found:`n$detailText"
}

Write-Host "No known direct or transitive vulnerabilities were found. Report: $ReportPath"
