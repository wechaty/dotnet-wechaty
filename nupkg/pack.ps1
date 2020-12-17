. ".\common.ps1"
# remove old nuget package
$packageFolder=Join-Path $packFolder package


if(Test-Path $packageFolder){
    $packages=get-childitem $packageFolder -force
    Foreach($package in $packages)
    {
	    $packagePath=$package.FullName
    
	    Remove-Item -Path $packagePath -Recurse -Force
        Write-Output $packagePath
    }
} else {
    md $packageFolder
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
    $localPath=Get-Location

    $releaseFolder= Join-Path $localPath "bin/Release"

    Write-Host  $releaseFolder
    if(Test-Path $releaseFolder) {
        Remove-Item -Recurse $releaseFolder
    }
    #md $releaseFolder

    Write-Host  $localPath
    dotnet msbuild  /p:Configuration=Release /p:SourceLinkCreate=true /t:pack

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