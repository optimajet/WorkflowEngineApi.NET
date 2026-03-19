param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("create", "remove")]
    [string]$Action,

    [Parameter(Mandatory=$true)]
    [string]$Name,

    [Parameter(Mandatory=$true)]
    [ValidateSet("mongo", "mssql", "mysql", "oracle", "postgres")]
    [string]$Provider,

    [Parameter(Mandatory=$false)]
    [int]$Port,

    [Parameter(Mandatory=$false)]
    [string[]]$Databases,

    [Parameter(Mandatory=$false)]
    [string]$User,

    [Parameter(Mandatory=$false)]
    [string]$Password
)

$ErrorActionPreference = "Stop"

function CheckExitCode()
{
    if ($LASTEXITCODE -ne 0)
    {
        throw "Build failed. Last exit code $LASTEXITCODE"
    }
}

function GetDefaultUser([string]$Provider)
{
    switch ($Provider)
    {
        "mssql" { return "SA" }
        "mysql" { return "root" }
        "postgres" { return "postgres" }
        default { return $null }
    }
}

function GetDefaultPassword([string]$Provider)
{
    switch ($Provider)
    {
        "mssql" { return "P@ssw0rd" }
        "mysql" { return "P@ssw0rd" }
        "oracle" { return "password" }
        "postgres" { return "P@ssw0rd" }
        default { return $null }
    }
}

function GetDefaultPort([string]$Provider)
{
    switch ($Provider)
    {
        "mongo" { return 27017 }
        "mssql" { return 1433 }
        "mysql" { return 3306 }
        "oracle" { return 1521 }
        "postgres" { return 5432 }
    }
}

function GetDefaultDatabases([string]$Provider)
{
    switch ($Provider)
    {
        "mongo" { return @("workflow-engine") }
        "mssql" { return @("workflow_engine") }
        "mysql" { return @("workflow_engine") }
        "oracle" { return @("WORKFLOW_ENGINE") }
        "postgres" { return @("workflow_engine") }
    }
}

function NewContainer([string]$Name, [string]$Provider, [int]$Port, [string[]]$Databases, [string]$User, [string]$Password)
{
    $resolvedProvider = $Provider.Trim()
    $resolvedPort = if ($Port -gt 0) { $Port } else { GetDefaultPort $resolvedProvider }
    $resolvedDatabases = if ($null -ne $Databases -and $Databases.Count -gt 0) { $Databases } else { GetDefaultDatabases $resolvedProvider }
    $resolvedUser = if ([string]::IsNullOrWhiteSpace($User)) { GetDefaultUser $resolvedProvider } else { $User }
    $resolvedPassword = if ([string]::IsNullOrWhiteSpace($Password)) { GetDefaultPassword $resolvedProvider } else { $Password }

    return [PSCustomObject]@{
        Name = $Name.Trim()
        Provider = $resolvedProvider
        Port = $resolvedPort
        Databases = $resolvedDatabases | ForEach-Object { $_.Trim() } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
        User = $resolvedUser
        Password = $resolvedPassword
    }
}

function CreateMongoContainer($Container)
{
    docker network create $Container.Name
    CheckExitCode

    docker run --name $( $Container.Name ) --network $Container.Name -p "$( $Container.Port ):27017" -d "mongo:7.0" mongod --replSet mongoReplicaSet --bind_ip "localhost,$( $Container.Name )"
    CheckExitCode

    Start-Sleep -Seconds 1
    docker exec $Container.Name mongosh --eval "rs.initiate({ _id: 'mongoReplicaSet', members: [ {_id: 0, host: '$( $Container.Name )'} ]})"
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection strings:" -ForegroundColor Yellow

    foreach ($Database in $Container.Databases)
    {
        Write-Host "mongodb://localhost:$( $Container.Port )/$( $Database )" -ForegroundColor Cyan
    }
}

function CreateMssqlContainer($Container)
{
    docker run --name $Container.Name -p "$( $Container.Port ):1433" -e "SA_PASSWORD=$( $Container.Password )" -e "ACCEPT_EULA=Y" -d mcr.microsoft.com/mssql/server:2022-latest
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow

    foreach ($Database in $Container.Databases)
    {
        Write-Host "Server=localhost,$( $Container.Port );Database=$( $Database );User Id=$( $Container.User );Password=$( $Container.Password );" -ForegroundColor Cyan
    }
}

function CreateMysqlContainer($Container)
{
    docker run --name $Container.Name -p "$( $Container.Port ):3306" -e "MYSQL_ROOT_HOST=%" -e "MYSQL_ROOT_PASSWORD=$( $Container.Password )" -d "mysql/mysql-server:8.0"
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow

    foreach ($Database in $Container.Databases)
    {
        Write-Host "Host=localhost;Port=$( $Container.Port );Database=$( $Database );User ID=$( $Container.User );Password=$( $Container.Password );" -ForegroundColor Cyan
    }
}

function CreateOracleContainer($Container)
{
    for ($i = 0; $i -lt $Container.Databases.Length; $i++)
    {
        $Database = $Container.Databases[$i]
        docker run --name "$( $Container.Name )_$( $i )" -p "$( $Container.Port + $i ):1521" -e "APP_USER=$( $Database )" -e "ORACLE_PASSWORD=$( $Container.Password )" -e "APP_USER_PASSWORD=$( $Container.Password )" -d gvenzl/oracle-free
        CheckExitCode
    }

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow

    for ($i = 0; $i -lt $Container.Databases.Length; $i++)
    {
        $Database = $Container.Databases[$i]
        Write-Host "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=$( $Container.Port + $i )))(CONNECT_DATA=(SERVICE_NAME=FREEPDB1)));User Id=$( $Database );Password=$( $Container.Password );" -ForegroundColor Cyan
    }
}

function CreatePostgresContainer($Container)
{
    docker run --name $Container.Name -p "$( $Container.Port ):5432" -e "POSTGRES_PASSWORD=$( $Container.Password )" -d "postgres:16.4"
    CheckExitCode

    Write-Host "$( $Container.Name ) container created, connection string:" -ForegroundColor Yellow

    foreach ($Database in $Container.Databases)
    {
        Write-Host "Host=localhost;Port=$( $Container.Port );Database=$( $Database );User Id=$( $Container.User );Password=$( $Container.Password );" -ForegroundColor Cyan
    }
}

function CreateContainer($Container)
{
    Write-Host "Creating $( $Container.Name ) container" -ForegroundColor Yellow

    switch ($Container.Provider)
    {
        "mongo" { CreateMongoContainer $Container }
        "mssql" { CreateMssqlContainer $Container }
        "mysql" { CreateMysqlContainer $Container }
        "oracle" { CreateOracleContainer $Container }
        "postgres" { CreatePostgresContainer $Container }
    }
}

function InitMongoContainer($Container)
{
    $IndexesScriptPath = Join-Path $PSScriptRoot "MongoIndexes.js"

    docker cp $IndexesScriptPath "$( $Container.Name ):./indexes.js"
    CheckExitCode

    for ($i = 0; $i -lt 10; $i++)
    {
        docker exec $Container.Name mongosh --eval "db.adminCommand('ping')"

        if ($LASTEXITCODE -ne 0)
        {
            Write-Host "Waiting for $( $Container.Name ) container to become healthy..." -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
        else
        {
            Write-Host "Initializing $( $Container.Name ) container databases" -ForegroundColor Yellow

            foreach ($Database in $Container.Databases)
            {
                docker exec $Container.Name mongosh "$( $Database )" --eval "load('/indexes.js')"
                CheckExitCode
            }

            return
        }
    }

    throw "Failed to initialize $( $Container.Name ) container"
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
            Write-Host "Initializing $( $Container.Name ) container databases" -ForegroundColor Yellow

            foreach ($Database in $Container.Databases)
            {
                docker exec $Container.Name /opt/mssql-tools18/bin/sqlcmd -S localhost -U $Container.User -C -P $Container.Password -Q "CREATE DATABASE $( $Database );"
                CheckExitCode
            }

            return
        }
    }

    throw "Failed to initialize $( $Container.Name ) container"
}

function InitMysqlContainer($Container)
{
    for ($i = 0; $i -lt 30; $i++)
    {
        docker exec $Container.Name mysql "-u$( $Container.User )" "-p$( $Container.Password )" -e "SELECT 1"

        if ($LASTEXITCODE -ne 0)
        {
            Write-Host "Waiting for $( $Container.Name ) container to become healthy..." -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
        else
        {
            Write-Host "Initializing $( $Container.Name ) container databases" -ForegroundColor Yellow

            foreach ($Database in $Container.Databases)
            {
                docker exec $Container.Name mysql "-u$( $Container.User )" "-p$( $Container.Password )" -e "CREATE DATABASE IF NOT EXISTS $( $Database );"
                CheckExitCode
            }

            return
        }
    }

    throw "Failed to initialize $( $Container.Name ) container"
}

function InitOracleContainer($Container)
{
    for ($i = 0; $i -lt $Container.Databases.Length; $i++)
    {
        $ContainerName = "$( $Container.Name )_$( $i )"
        $Database = $Container.Databases[$i]

        for ($j = 0; $j -lt 30; $j++)
        {
            docker exec $ContainerName sqlplus "$( $Database )/$( $Container.Password )@localhost:1521/FREEPDB1" "SELECT 1 FROM dual" 2>$null

            if ($LASTEXITCODE -ne 0)
            {
                Write-Host "Waiting for $( $ContainerName ) container to become healthy... ($j/30)" -ForegroundColor Yellow
                Start-Sleep -Seconds 5
            }
            else
            {
                Write-Host "$( $ContainerName ) container is ready!" -ForegroundColor Green
                break
            }

            if ($j -eq 29)
            {
                throw "Failed to initialize $( $ContainerName ) container"
            }
        }
    }
}

function InitPostgresContainer($Container)
{
    for ($i = 0; $i -lt 30; $i++)
    {
        docker exec $Container.Name psql -U $Container.User -d postgres -c "SELECT 1"

        if ($LASTEXITCODE -ne 0)
        {
            Write-Host "Waiting for $( $Container.Name ) container to become healthy..." -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
        else
        {
            Write-Host "Initializing $( $Container.Name ) container databases" -ForegroundColor Yellow

            foreach ($Database in $Container.Databases)
            {
                docker exec $Container.Name psql -U $Container.User -d postgres -c "CREATE DATABASE $( $Database );"
                CheckExitCode
            }

            return
        }
    }

    throw "Failed to initialize $( $Container.Name ) container"
}

function InitContainer($Container)
{
    Write-Host "Initializing $( $Container.Name ) container" -ForegroundColor Yellow

    switch ($Container.Provider)
    {
        "mongo" { InitMongoContainer $Container }
        "mssql" { InitMssqlContainer $Container }
        "mysql" { InitMysqlContainer $Container }
        "oracle" { InitOracleContainer $Container }
        "postgres" { InitPostgresContainer $Container }
    }
}

function RemoveMongoContainer($Container)
{
    docker kill $Container.Name
    docker rm $Container.Name
    docker network rm $Container.Name
}

function RemoveDockerContainer($Container)
{
    docker kill $Container.Name
    docker rm $Container.Name
}

function RemoveOracleContainer($Container)
{
    $ContainerNames = docker ps -a --format "{{.Names}}" | Where-Object { $_ -like "$( $Container.Name )_*" }
    CheckExitCode

    foreach ($ContainerName in $ContainerNames)
    {
        docker kill $ContainerName
        docker rm $ContainerName
    }
}

function RemoveContainer($Container)
{
    Write-Host "Removing $( $Container.Name ) container" -ForegroundColor Yellow

    switch ($Container.Provider)
    {
        "mongo" { RemoveMongoContainer $Container }
        "mssql" { RemoveDockerContainer $Container }
        "mysql" { RemoveDockerContainer $Container }
        "oracle" { RemoveOracleContainer $Container }
        "postgres" { RemoveDockerContainer $Container }
    }
}

$Container = NewContainer -Name $Name -Provider $Provider -Port $Port -Databases $Databases -User $User -Password $Password

if ($Container.Databases.Count -eq 0)
{
    throw "At least one database name must be provided"
}

if ($PSVersionTable.OS -match "ARM" -and $Container.Provider -eq "oracle")
{
    throw "Oracle container is not supported on ARM processor architecture"
}

switch ($Action)
{
    "create"
    {
        CreateContainer $Container
        Write-Host "Container $( $Container.Name ) created, waiting for it to become healthy..." -ForegroundColor Yellow
        InitContainer $Container
        Write-Host "Initializing completed, container $( $Container.Name ) available!" -ForegroundColor Green
    }
    "remove"
    {
        RemoveContainer $Container
        Write-Host "Removing completed, container $( $Container.Name ) removed!" -ForegroundColor Green
    }
}
