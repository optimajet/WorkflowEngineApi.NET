param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Mongo", "Mssql", "Mysql", "Oracle", "Postgres")]
    [string[]]$Providers=@("Mongo", "Mssql", "Mysql", "Oracle", "Postgres")
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

function AssertTestsWereExecuted([string]$ResultsDirectory, [string]$ProjectName)
{
    $TrxFile = Get-ChildItem $ResultsDirectory -Filter *.trx -File | Sort-Object LastWriteTime -Descending | Select-Object -First 1

    if ($null -eq $TrxFile)
    {
        throw "No trx file was generated for $( $ProjectName )"
    }

    [xml]$Trx = Get-Content -Raw $TrxFile.FullName
    $Counters = $Trx.TestRun.ResultSummary.Counters
    $Total = [int]$Counters.total
    $Executed = [int]$Counters.executed

    if ($Total -eq 0 -or $Executed -eq 0)
    {
        throw "No tests were discovered or executed for $( $ProjectName ). See $( $TrxFile.FullName )"
    }
}

function RunTestsProject([string]$ProjectPath, [string]$ProvidersString)
{
    $ProjectDir = Split-Path $ProjectPath -Parent
    $ProjectName = Split-Path $ProjectPath -Leaf
    $ResultsDirectory = Join-Path $ProjectDir "TestResults\run-tests-$( [Guid]::NewGuid().ToString('N') )"

    Push-Location $ProjectDir

    try
    {
        Write-Host "Running tests for $( $ProjectName )..." -ForegroundColor Yellow
        $env:RUNNING_PROVIDERS = $ProvidersString
        dotnet test $ProjectPath --results-directory $ResultsDirectory --logger html --logger "console;verbosity=normal" --logger trx --blame --blame-hang-timeout 10m
        CheckExitCode
        AssertTestsWereExecuted -ResultsDirectory $ResultsDirectory -ProjectName $ProjectName
        Write-Host "Tests for $( $ProjectName ) completed successfully" -ForegroundColor Green
    }
    finally
    {
        Pop-Location
    }
}

$TestProjects = @(
    "$( $PSScriptRoot )/../../WorkflowApi.Client.Data.Test/WorkflowApi.Client.Data.Test.csproj",
    "$( $PSScriptRoot )/../../WorkflowApi.Client.Rpc.Test/WorkflowApi.Client.Rpc.Test.csproj"
)

Invoke-Expression "$( $PSScriptRoot )/remove-containers.ps1"
Invoke-Expression "$( $PSScriptRoot )/create-containers.ps1 -Providers $( $ProvidersString )"

try
{
    foreach ($TestProject in $TestProjects)
    {
        RunTestsProject -ProjectPath $TestProject -ProvidersString $ProvidersString
    }
}
finally
{
    Invoke-Expression "$( $PSScriptRoot )/remove-containers.ps1 -Providers $( $ProvidersString )"
    Write-Host "Execution completed" -ForegroundColor Yellow
}
