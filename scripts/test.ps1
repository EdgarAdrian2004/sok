param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root "SOK.sln"

Write-Host "Ejecutando pruebas de SOK ($Configuration)..." -ForegroundColor Cyan
dotnet test $solution -c $Configuration --nologo

if ($LASTEXITCODE -ne 0) {
    throw "Las pruebas fallaron con código $LASTEXITCODE."
}

Write-Host "Pruebas completadas." -ForegroundColor Green
