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
Invoke-Expression "$( $PSScriptRoot )/create-containers.ps1"

Push-Location $TestsDir

try
{
    Write-Host "Running tests..." -ForegroundColor Yellow
    dotnet test --logger html --logger "console;verbosity=detailed" --logger trx --blame --blame-hang-timeout 10m
    CheckExitCode
    Write-Host "Tests completed successfully" -ForegroundColor Green
}
finally
{
    Pop-Location
    Invoke-Expression "$( $PSScriptRoot )/remove-containers.ps1"
    Write-Host "Execution completed" -ForegroundColor Yellow
}
