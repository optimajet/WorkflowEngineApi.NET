param(
    [Parameter(Mandatory=$false)]
    [string[]]$Providers=@("Mongo", "Mssql", "Mysql", "Oracle", "Postgres", "Sqlite")
)

$ErrorActionPreference = "Stop"
$Providers = $Providers | ForEach-Object { $_.Trim() }

function CheckExitCode()
{
    if ($LASTEXITCODE -ne 0)
    {
        throw "Build failed. Last exit code $LASTEXITCODE"
    }
}

$Root = "$( $PSScriptRoot )/../.."

function CreateMongoContainer($Container)
{
    docker network create $Container.Name
    CheckExitCode
    docker run --name $( $Container.Name ) --network $Container.Name -p "$( $Container.Port ):27017" -d "mongo:7.0" mongod --replSet mongoReplicaSet --bind_ip "localhost,$( $Container.Name )"
    CheckExitCode

    Start-Sleep -Seconds 1
    docker exec $Container.Name mongosh --eval "rs.initiate({ _id: 'mongoReplicaSet', members: [ {_id: 0, host: '$( $Container.Name )'} ]})"
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow
    Write-Host "mongodb://localhost:$( $Container.Port )/$( $Container.Database )" -ForegroundColor Cyan
}

function CreateMssqlContainer($Container)
{
    docker run --name $Container.Name -p "$( $Container.Port ):1433" -e "SA_PASSWORD=$( $Container.Password )" -e "ACCEPT_EULA=Y" -d mcr.microsoft.com/mssql/server:2022-latest
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow
    Write-Host "Server=localhost,$( $Container.Port );Database=master;User Id=$( $Container.User );Password=$( $Container.Password );" -ForegroundColor Cyan
}

function CreateMysqlContainer($Container)
{
    docker run --name $Container.Name -p "$( $Container.Port ):3306" -e "MYSQL_ROOT_HOST=%" -e "MYSQL_ROOT_PASSWORD=$( $Container.Password )" -d "mysql/mysql-server:8.0"
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow
    Write-Host "Host=localhost;Port=$( $Container.Port );Database=$( $Container.Database );User ID=$( $Container.User );Password=$( $Container.Password );" -ForegroundColor Cyan
}

function CreateOracleContainer($Container)
{
    docker run --name $Container.Name -p "$( $Container.Port ):1521" -e "APP_USER=$( $Container.Database )" -e "ORACLE_PASSWORD=$( $Container.Password )" -e "APP_USER_PASSWORD=$( $Container.Password )" -d gvenzl/oracle-free
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow
    Write-Host "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=$( $Container.Port )))(CONNECT_DATA=(SERVICE_NAME=FREEPDB1)));User Id=$( $Container.Database );Password=$( $Container.Password );" -ForegroundColor Cyan
}

function CreatePostgresContainer($Container)
{
    docker run --name $Container.Name -p "$( $Container.Port ):5432" -e "POSTGRES_PASSWORD=$( $Container.Password )" -d "postgres:16.4"
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow
    Write-Host "Host=localhost;Port=$( $Container.Port );Database=postgres;User Id=$( $Container.User );Password=$( $Container.Password );" -ForegroundColor Cyan
}

function CreateContainer($Container)
{
    Write-Host "Creating $( $Container.Name ) container" -ForegroundColor Yellow

    switch ($Container.Provider)
    {
        "Mongo" {
            CreateMongoContainer $Container
        }
        "Mssql" {
            CreateMssqlContainer $Container
        }
        "Mysql" {
            CreateMysqlContainer $Container
        }
        "Oracle" {
            CreateOracleContainer $Container
        }
        "Postgres" {
            CreatePostgresContainer $Container
        }
    }
}


function InitMongoContainer($Container)
{
    $IndexesScriptPath = "$( $Root )/Scripts/Providers/MongoIndexes.js"

    docker cp $IndexesScriptPath "$( $Container.Name ):./indexes.js"

    for ($i = 0; $i -lt 10; $i++)
    {
        docker exec $Container.Name mongosh "$( $Container.Database )" --eval "load('/indexes.js')"

        if ($LASTEXITCODE -ne 0)
        {
            Write-Host "Waiting for $( $Container.Name ) container to become healthy..." -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
        else
        {
            break
        }

        if ($i -eq 9)
        {
            throw "Failed to initialize $( $Container.Name ) container"
        }
    }
}

function InitMssqlContainer($Container)
{
    for ($i = 0; $i -lt 30; $i++)
    {
        docker exec $Container.Name /opt/mssql-tools18/bin/sqlcmd -S localhost -U $Container.User -C -P $Container.Password -Q "SELECT 1"

        if ($LASTEXITCODE -ne 0)
        {
            Write-Host "Waiting for $( $Container.Name ) container to become healthy..." -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
        else
        {
            return
        }

    }
    throw "Failed to initialize $( $Container.Name ) container"
}

function InitMysqlContainer($Container)
{
    for ($i = 0; $i -lt 30; $i++)
    {
        docker exec $Container.Name mysql "-u$( $Container.User )" "-p$( $Container.Password )" -e "CREATE DATABASE IF NOT EXISTS $( $Container.Database );"

        if ($LASTEXITCODE -ne 0)
        {
            Write-Host "Waiting for $( $Container.Name ) container to become healthy..." -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
        else
        {
            return
        }

    }
    throw "Failed to initialize $( $Container.Name ) container"
}

function InitOracleContainer($Container)
{

}

function InitPostgresContainer($Container)
{

}

function InitContainer($Container)
{
    Write-Host "Initializing $( $Container.Name ) container" -ForegroundColor Yellow

    switch ($Container.Provider)
    {
        "Mongo" {
            InitMongoContainer $Container
        }
        "Mssql" {
            InitMssqlContainer $Container
        }
        "Mysql" {
            InitMysqlContainer $Container
        }
        "Oracle" {
            InitOracleContainer $Container
        }
        "Postgres" {
            InitPostgresContainer $Container
        }
    }
}


Write-Host "Creating docker containers" -ForegroundColor Yellow

$Containers = Get-Content -Raw "$( $PSScriptRoot )/containers.json" | ConvertFrom-Json

if ($PSVersionTable.OS -match "ARM")
{
    Write-Host "Disabling Oracle container for ARM processor architecture" -ForegroundColor Red
    $Providers = $Providers | Where-Object { $_ -ne "Oracle" }
    Write-Host "Disabling Sqlite container for ARM processor architecture" -ForegroundColor Red
    $Providers = $Providers | Where-Object { $_ -ne "Sqlite" }
}

foreach ($Container in $Containers)
{
    if ($Providers -contains $Container.Provider)
    {
        CreateContainer $Container
    }
    else
    { 
        Write-Host "Container $( $Container.Name ) is skipped" -ForegroundColor Magenta
    }
}

Write-Host "Containers created, waiting for them to become healthy..." -ForegroundColor Yellow

Write-Host "Initializing docker containers" -ForegroundColor Yellow

foreach ($Container in $Containers)
{
    if ($Providers -contains $Container.Provider)
    {
        InitContainer $Container
    }
    else
    {
        Write-Host "Container $( $Container.Name ) is skipped" -ForegroundColor Magenta
    }
}

Write-Host "Initializing completed, containers available!" -ForegroundColor Green
