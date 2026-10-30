param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root "SOK.sln"

Write-Host "Compilando SOK ($Configuration)..." -ForegroundColor Cyan
dotnet build $solution -c $Configuration --nologo

if ($LASTEXITCODE -ne 0) {
    throw "La compilación falló con código $LASTEXITCODE."
}

Write-Host "Compilación completada." -ForegroundColor Green
