param([string]$StackName = 'sakila')

$ErrorActionPreference = 'Stop'
docker node ls
docker stack services $StackName
docker stack ps $StackName --no-trunc
docker service ps "${StackName}_sakilaapp" --no-trunc
docker service ps "${StackName}_postgres" --no-trunc

try {
    $response = Invoke-WebRequest -Uri 'http://localhost:5164' -UseBasicParsing -TimeoutSec 15
    Write-Host "HTTP http://localhost:5164 = $($response.StatusCode)"
} catch {
    Write-Warning "La aplicación todavía no respondió: $($_.Exception.Message)"
}
