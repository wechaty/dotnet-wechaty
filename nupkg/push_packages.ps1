. ".\common.ps1"


$apiKey = $args[0]
Write-Output $apiKey;

# Get the version
[xml]$commonPropsXml = Get-Content (Join-Path $rootFolder "src/common.props")
$version = $commonPropsXml.Project.PropertyGroup.Version

Write-Output $projects

# Publish all packages
foreach($project in $projects) {
    $projectName = $project.Substring($project.LastIndexOf("/") + 1)
    $packagePath= Join-Path "./package" ($projectName + "." + $version + ".nupkg")
    Write-Output $packagePath
    dotnet nuget push $packagePath -s https://api.nuget.org/v3/index.json --api-key "$apiKey"
}


# Go back to the pack folder
Set-Location $packFolder
