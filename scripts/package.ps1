param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "src\SOK.UI\SOK.UI.csproj"
$output = Join-Path $root "artifacts\publish"

Write-Host "Publicando SOK ($Configuration, $Runtime)..." -ForegroundColor Cyan
dotnet publish $project -c $Configuration -r $Runtime --self-contained false -o $output --nologo

if ($LASTEXITCODE -ne 0) {
    throw "La publicación falló con código $LASTEXITCODE."
}

Write-Host "Salida: $output" -ForegroundColor Green
