param(
    [string]$StackName = 'sakila',
    [string]$PostgresNode = ''
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$envFile = Join-Path $projectRoot '.env'
$stackFile = Join-Path $projectRoot 'docker-stack.yml'

if (-not (Test-Path $envFile)) {
    throw 'Falta .env. Créelo localmente a partir de .env.example.'
}

$nodes = @(docker node ls --format '{{.Hostname}}|{{.Status}}|{{.ManagerStatus}}')
$readyNodes = @($nodes | Where-Object { $_ -match '\|Ready\|' })
if ($readyNodes.Count -lt 2) {
    throw 'Se requieren al menos dos nodos Ready. No se desplegará una práctica multinodo ficticia.'
}

if ([string]::IsNullOrWhiteSpace($PostgresNode)) {
    $PostgresNode = ($nodes | Where-Object { $_ -match '\|Ready\|Leader$' } | Select-Object -First 1).Split('|')[0]
}

docker node inspect $PostgresNode | Out-Null
docker node update --label-add sakila.postgres-data=true $PostgresNode | Out-Null

$envValues = @{}
Get-Content $envFile | ForEach-Object {
    if ($_ -match '^([^#][^=]*)=(.*)$') {
        $name = $Matches[1].Trim()
        $value = $Matches[2]
        if ($value.StartsWith("'") -and $value.EndsWith("'")) {
            $value = $value.Substring(1, $value.Length - 2).Replace("\'", "'")
        }
        $envValues[$name] = $value
    }
}

$secretMap = [ordered]@{
    postgres_password    = 'POSTGRES_PASSWORD'
    paypal_client_id     = 'PAYPAL_CLIENT_ID'
    paypal_client_secret = 'PAYPAL_CLIENT_SECRET'
    payphone_token       = 'PAYPHONE_TOKEN'
    payphone_store_id    = 'PAYPHONE_STORE_ID'
}

foreach ($item in $secretMap.GetEnumerator()) {
    if ([string]::IsNullOrWhiteSpace($envValues[$item.Value])) {
        throw "Falta $($item.Value) en .env."
    }

    $exists = docker secret ls --filter "name=$($item.Key)" --format '{{.Name}}'
    if (-not $exists) {
        $envValues[$item.Value] | docker secret create $item.Key - | Out-Null
    }
}

docker stack deploy --detach=true --resolve-image never -c $stackFile $StackName
Write-Host 'Stack solicitado. Use 05-Verify-Swarm.ps1 para esperar y verificar el estado real.'
