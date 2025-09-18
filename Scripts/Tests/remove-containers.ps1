param(
    [Parameter(Mandatory=$false)]
    [string[]]$Providers=@("Mongo", "Mssql", "Mysql", "Oracle", "Postgres", "Sqlite")
)

$ErrorActionPreference = "Stop"
$Providers = $Providers | ForEach-Object { $_.Trim() }

function RemoveMongoContainer($Container) {
    Write-Host "Removing $( $Container.Name ) container" -ForegroundColor Yellow

    docker kill $Container.Name
    docker rm $Container.Name

    docker network rm $Container.Name
}

function RemoveDockerContainer ($Container) {
    Write-Host "Removing $( $Container.Name ) container" -ForegroundColor Yellow
    docker kill $Container.Name
    docker rm $Container.Name
}

function RemoveOracleContainer ($Container) {
    Write-Host "Removing $( $Container.Name ) container" -ForegroundColor Yellow

    for ($i = 0; $i -lt $Container.Databases.Length; $i++)
    {
        docker kill "$( $Container.Name )_$( $i )"
        docker rm "$( $Container.Name )_$( $i )"
    }
}

function RemoveContainer($Container) {
    Write-Host "Removing $( $Container.Name ) container" -ForegroundColor Yellow

    switch ($Container.Provider) {
        "Mongo" {
            RemoveMongoContainer $Container
        }
        "Mssql" {
            RemoveDockerContainer $Container
        }
        "Mysql" {
            RemoveDockerContainer $Container
        }
        "Oracle" {
            RemoveOracleContainer $Container
        }
        "Postgres" {
            RemoveDockerContainer $Container
        }
    }
}

$Containers = Get-Content -Raw "$( $PSScriptRoot )/containers.json" | ConvertFrom-Json

Write-Host "Removing docker containers" -ForegroundColor Yellow

foreach ($Container in $Containers) {
    if ($Providers -contains $Container.Provider)
    {
        RemoveContainer $Container
    }
    else
    {
        Write-Host "Container $( $Container.Name ) is skipped" -ForegroundColor Magenta
    }
}

Write-Host "Removing completed, containers removed!" -ForegroundColor Green