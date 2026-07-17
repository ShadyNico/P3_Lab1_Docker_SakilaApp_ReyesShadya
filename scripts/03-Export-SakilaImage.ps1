param(
    [string]$OutputPath = (Join-Path $PWD 'sakilaapp-compose.tar')
)

$ErrorActionPreference = 'Stop'
docker image inspect sakilaapp:compose | Out-Null
docker image save --output $OutputPath sakilaapp:compose

Write-Host "Imagen exportada en: $OutputPath"
Write-Host 'Copie el archivo a cada worker y ejecute allí:'
Write-Host "docker image load --input `"$OutputPath`""
