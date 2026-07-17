param([string]$StackName = 'sakila')

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$backupPath = Join-Path $projectRoot 'backups\sakila.dump'

if (-not (Test-Path $backupPath)) {
    throw 'Falta backups/sakila.dump. Ejecute primero 11-Backup-HostPostgres.ps1.'
}

$containerId = docker ps `
    --filter "label=com.docker.swarm.service.name=${StackName}_postgres" `
    --format '{{.ID}}' | Select-Object -First 1

if (-not $containerId) {
    throw 'La réplica PostgreSQL no está en este nodo o todavía no está Running.'
}

docker cp $backupPath "${containerId}:/tmp/sakila.dump"
docker exec $containerId pg_restore `
    --username cruduser --dbname sakila --exit-on-error /tmp/sakila.dump

Write-Host 'Base Sakila restaurada. El archivo temporal queda dentro del contenedor y no contiene credenciales.'
