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
            RemoveDockerContainer $Container
        }
        "Postgres" {
            RemoveDockerContainer $Container
        }
    }
}

$Containers = Get-Content -Raw "$( $PSScriptRoot )/containers.json" | ConvertFrom-Json
$Excluded = @('Mongo', 'Mysql', 'Oracle', 'Postgres', 'Sqlite')

Write-Host "Removing docker containers" -ForegroundColor Yellow

foreach ($Container in $Containers) {
    if ($Excluded -contains $Container.Provider) {
        Write-Host "Container $( $Container.Name ) is excluded" -ForegroundColor Magenta
        continue
    }

    RemoveContainer $Container
}

Write-Host "Removing completed, containers removed!" -ForegroundColor Green