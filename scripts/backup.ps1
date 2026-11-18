param(
    [string]$BackupRoot = "C:\backups",
    [int]$KeepDays = 14
)

$ErrorActionPreference = "Stop"

function Invoke-Native {
    param([scriptblock]$Command)
    & $Command
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code ${LASTEXITCODE}: $Command"
    }
}

$timestamp = Get-Date -Format "yyyy-MM-dd_HHmmss"
$target = Join-Path $BackupRoot $timestamp
New-Item -ItemType Directory -Path $target | Out-Null

Invoke-Native { docker exec household-db pg_dump -U household -Fc -f /tmp/household.dump household }
Invoke-Native { docker cp household-db:/tmp/household.dump (Join-Path $target "household.dump") }
Invoke-Native { docker exec household-db rm /tmp/household.dump }

New-Item -ItemType Directory -Path (Join-Path $target "uploads") | Out-Null
Invoke-Native { docker cp household-app:/data/uploads/. (Join-Path $target "uploads") }

Get-ChildItem -Path $BackupRoot -Directory |
    Where-Object { $_.Name -match '^\d{4}-\d{2}-\d{2}_\d{6}$' -and $_.LastWriteTime -lt (Get-Date).AddDays(-$KeepDays) } |
    Remove-Item -Recurse -Force

Write-Host "Backup completed: $target"
