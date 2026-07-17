param([string]$StackName = 'sakila')

$ErrorActionPreference = 'Stop'
docker service scale "${StackName}_sakilaapp=3"
docker service ps "${StackName}_sakilaapp"
docker service ls
