[CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'High')]
param([string]$StackName = 'sakila')

$ErrorActionPreference = 'Stop'
if ($PSCmdlet.ShouldProcess($StackName, 'Eliminar el stack Docker Swarm')) {
    docker stack rm $StackName
}
