param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("mongo", "mssql", "mysql", "oracle", "postgres")]
    [string[]]$Providers=@("mongo", "mssql", "mysql", "oracle", "postgres")
)

$ErrorActionPreference = "Stop"
$Providers = $Providers | ForEach-Object { $_.Trim() }
$ManageContainerScript = Join-Path $PSScriptRoot "manage-container.ps1"

Write-Host "Removing docker containers" -ForegroundColor Yellow

if ($Providers -contains "mongo")
{
    & $ManageContainerScript -Action remove -Name "wfe-api-mongo" -Provider "mongo"
}
else
{
    Write-Host "Container wfe-api-mongo is skipped" -ForegroundColor Magenta
}

if ($Providers -contains "mssql")
{
    & $ManageContainerScript -Action remove -Name "wfe-api-mssql" -Provider "mssql"
}
else
{
    Write-Host "Container wfe-api-mssql is skipped" -ForegroundColor Magenta
}

if ($Providers -contains "mysql")
{
    & $ManageContainerScript -Action remove -Name "wfe-api-mysql" -Provider "mysql"
}
else
{
    Write-Host "Container wfe-api-mysql is skipped" -ForegroundColor Magenta
}

if ($Providers -contains "oracle")
{
    & $ManageContainerScript -Action remove -Name "wfe-api-oracle" -Provider "oracle"
}
else
{
    Write-Host "Container wfe-api-oracle is skipped" -ForegroundColor Magenta
}

if ($Providers -contains "postgres")
{
    & $ManageContainerScript -Action remove -Name "wfe-api-postgres" -Provider "postgres"
}
else
{
    Write-Host "Container wfe-api-postgres is skipped" -ForegroundColor Magenta
}

Write-Host "Removing completed, containers removed!" -ForegroundColor Green
