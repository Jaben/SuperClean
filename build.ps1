#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for SuperClean.

.DESCRIPTION
    Builds, publishes a single-file win-x64 exe, and/or packs the dotnet tool.

.EXAMPLE
    ./build.ps1                 # restore + build (Release)
    ./build.ps1 -Target Publish # single-file exe -> ./publish/SuperClean.exe
    ./build.ps1 -Target Pack    # nupkg (dotnet tool) -> ./artifacts
    ./build.ps1 -Target All     # build + publish + pack
    ./build.ps1 -Target Clean
#>
[CmdletBinding()]
param(
    [ValidateSet('Build', 'Publish', 'Pack', 'All', 'Clean')]
    [string]$Target = 'Build',

    [string]$Configuration = 'Release',

    [string]$Runtime = 'win-x64'
)

$ErrorActionPreference = 'Stop'

$root = $PSScriptRoot
$project = Join-Path $root 'src/SuperClean.csproj'
$publishDir = Join-Path $root 'publish'
$artifactsDir = Join-Path $root 'artifacts'

function Invoke-Step([string]$Name, [scriptblock]$Action) {
    Write-Host "==> $Name" -ForegroundColor Cyan
    & $Action
    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE"
    }
}

switch ($Target) {
    'Clean' {
        Invoke-Step 'Clean' { dotnet clean $project -c $Configuration --nologo }
        Remove-Item $publishDir, $artifactsDir -Recurse -Force -ErrorAction Ignore
        break
    }
    default {
        Invoke-Step 'Build' { dotnet build $project -c $Configuration --nologo }

        if ($Target -in 'Publish', 'All') {
            Invoke-Step 'Publish' { dotnet publish $project -c $Configuration -r $Runtime -o $publishDir --nologo }
            Write-Host "Published: $(Join-Path $publishDir 'SuperClean.exe')" -ForegroundColor Green
        }

        if ($Target -in 'Pack', 'All') {
            Invoke-Step 'Pack' { dotnet pack $project -c $Configuration -o $artifactsDir --nologo }
            Write-Host "Packed to: $artifactsDir" -ForegroundColor Green
        }
    }
}

Write-Host 'Done.' -ForegroundColor Green
