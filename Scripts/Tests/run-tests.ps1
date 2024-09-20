param(
    [Parameter(Mandatory=$false)]
    [string[]]$Providers=@("Mongo", "Mssql", "Mysql", "Oracle", "Postgres", "Sqlite")
)

$Providers = $Providers | ForEach-Object { $_.Trim() }
$ProvidersString = $Providers -join ','
$ErrorActionPreference = "Stop"

function CheckExitCode()
{
    if ($LASTEXITCODE -ne 0)
    {
        throw "Build failed. Last exit code $LASTEXITCODE"
    }
}

$TestsDir = "$( $PSScriptRoot )/../../WorkflowApi.Client.Test"

Invoke-Expression "$( $PSScriptRoot )/remove-containers.ps1"
Invoke-Expression "$( $PSScriptRoot )/create-containers.ps1 -Providers $( $ProvidersString )"

Push-Location $TestsDir

try
{
    Write-Host "Running tests..." -ForegroundColor Yellow
    $env:RUNNING_CONFIGURATIONS=$( $ProvidersString )
    dotnet test --logger html --logger "console;verbosity=normal" --logger trx --blame --blame-hang-timeout 10m
    CheckExitCode
    Write-Host "Tests completed successfully" -ForegroundColor Green
}
finally
{
    Pop-Location
    Invoke-Expression "$( $PSScriptRoot )/remove-containers.ps1 -Providers $( $ProvidersString )"
    Write-Host "Execution completed" -ForegroundColor Yellow
}
