param(
    [string]$HostName = 'host.docker.internal',
    [int]$Port = 5432,
    [string]$Database = 'sakila',
    [string]$Username = 'cruduser'
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$envFile = Join-Path $projectRoot '.env'
$backupDirectory = Join-Path $projectRoot 'backups'
$backupPath = Join-Path $backupDirectory 'sakila.dump'

if (-not (Test-Path $envFile)) {
    throw 'Falta .env.'
}

$passwordLine = Get-Content $envFile | Where-Object { $_ -match '^POSTGRES_PASSWORD=' } | Select-Object -First 1
if (-not $passwordLine) {
    throw 'Falta POSTGRES_PASSWORD en .env.'
}

$password = $passwordLine.Substring($passwordLine.IndexOf('=') + 1)
if ($password.StartsWith("'") -and $password.EndsWith("'")) {
    $password = $password.Substring(1, $password.Length - 2).Replace("\'", "'")
}

New-Item -ItemType Directory -Path $backupDirectory -Force | Out-Null
$env:PGPASSWORD = $password
try {
    docker run --rm --env PGPASSWORD `
        --mount "type=bind,source=$backupDirectory,target=/backup" `
        postgres:18-alpine `
        pg_dump --host $HostName --port $Port --username $Username `
        --dbname $Database --format custom --file /backup/sakila.dump
    if ($LASTEXITCODE -ne 0) {
    throw "El respaldo falló y no se generó un archivo válido."
    }
} finally {
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
    Remove-Variable password -ErrorAction SilentlyContinue
}

if (-not (Test-Path $backupPath)) {
    throw 'No se generó el respaldo.'
}
Write-Host "Respaldo lógico creado en $backupPath (ignorado por Git y Docker)."
