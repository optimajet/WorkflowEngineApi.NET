param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("mongo", "mssql", "mysql", "oracle", "postgres")]
    [string[]]$Providers=@("mongo", "mssql", "mysql", "oracle", "postgres")
)

$ErrorActionPreference = "Stop"
$Providers = $Providers | ForEach-Object { $_.Trim() }
$ManageContainerScript = Join-Path $PSScriptRoot "manage-container.ps1"

Write-Host "Creating docker containers" -ForegroundColor Yellow

if ($PSVersionTable.OS -match "ARM")
{
    Write-Host "Disabling oracle container for ARM processor architecture" -ForegroundColor Red
    $Providers = $Providers | Where-Object { $_ -ne "oracle" }
}

if ($Providers -contains "mongo")
{
    & $ManageContainerScript -Action create -Name "wfe-api-mongo" -Provider "mongo" -Port 47017 -Databases @("data-tests", "rpc-tests", "workflow-engine")
}
else
{
    Write-Host "Container wfe-api-mongo is skipped" -ForegroundColor Magenta
}

if ($Providers -contains "mssql")
{
    & $ManageContainerScript -Action create -Name "wfe-api-mssql" -Provider "mssql" -Port 41433 -User "SA" -Password "P@ssw0rd" -Databases @("data_tests", "rpc_tests", "workflow_engine")
}
else
{
    Write-Host "Container wfe-api-mssql is skipped" -ForegroundColor Magenta
}

if ($Providers -contains "mysql")
{
    & $ManageContainerScript -Action create -Name "wfe-api-mysql" -Provider "mysql" -Port 43306 -User "root" -Password "P@ssw0rd" -Databases @("data_tests", "rpc_tests", "workflow_engine")
}
else
{
    Write-Host "Container wfe-api-mysql is skipped" -ForegroundColor Magenta
}

if ($Providers -contains "oracle")
{
    & $ManageContainerScript -Action create -Name "wfe-api-oracle" -Provider "oracle" -Port 41521 -Password "password" -Databases @("DATA_TESTS", "RPC_TESTS", "WORKFLOW_ENGINE")
}
else
{
    Write-Host "Container wfe-api-oracle is skipped" -ForegroundColor Magenta
}

if ($Providers -contains "postgres")
{
    & $ManageContainerScript -Action create -Name "wfe-api-postgres" -Provider "postgres" -Port 45432 -User "postgres" -Password "P@ssw0rd" -Databases @("data_tests", "rpc_tests", "workflow_engine")
}
else
{
    Write-Host "Container wfe-api-postgres is skipped" -ForegroundColor Magenta
}

Write-Host "Creating completed, containers available!" -ForegroundColor Green
