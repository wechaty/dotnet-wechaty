. ".\common.ps1"
# remove old nuget package
$packageFolder=Join-Path $packFolder package
$packages=get-childitem $packageFolder -force
Foreach($package in $packages)
{
	$packagePath=$package.FullName
    
	Remove-Item -Path $packagePath -Recurse -Force
    Write-Output $packagePath
}

# Rebuild all solutions
foreach($solution in $solutions) {
    $solutionFolder = Join-Path $rootFolder $solution
    Set-Location $solutionFolder
    & dotnet restore
}

# Create all packages
foreach($project in $projects) {
    
    $projectFolder = Join-Path $rootFolder $project

    # Create nuget pack
    Set-Location $projectFolder
    Remove-Item -Recurse (Join-Path $projectFolder "bin/Release")
    & dotnet msbuild /t:pack /p:Configuration=Release /p:SourceLinkCreate=true

    if (-Not $?) {
        Write-Host ("Packaging failed for the project: " + $projectFolder)
        exit $LASTEXITCODE
    }
    
    # Copy nuget package
    $projectName = $project.Substring($project.LastIndexOf("/") + 1)
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $projectName + ".*.nupkg")
    Move-Item $projectPackPath  $packageFolder
}

# Go back to the pack folder
Set-Location $packFolder