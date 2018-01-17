param (
    [string]
    $NuGetApiKey
)

Remove-Item $PSScriptRoot\wrk -Force -Recurse -ErrorAction SilentlyContinue
New-Item $PSScriptRoot\wrk\psXdtTransform -ItemType Directory

$releaseDirectory = (Resolve-Path $PSScriptRoot\..\src\psXdtTransform\bin\Release\)

Copy-Item "$releaseDirectory\*" $PSScriptRoot\wrk\psXdtTransform -Force

Publish-Module -Path $PSScriptRoot\wrk\psXdtTransform -NuGetApiKey $NuGetApiKey

Remove-Item $PSScriptRoot\wrk -Force -Recurse -ErrorAction SilentlyContinue