# 模块名称
$modulesNames = @("CMSMod", "FileManagerMod", "OrderMod", "CustomerMod")

# 移动模块到临时目录
function TempModule([string]$solutionPath, [string]$moduleName) {
    Write-Host "move module:"$moduleName
    # $tmp = Join-Path $solutionPath "./.tmp"
    # $entityPath = Join-Path $solutionPath "./src/Definition/Entity" $moduleName

    # $destDir = Join-Path $tmp $moduleName

    # # move entity to tmp   
    # $entityDestDir = Join-Path $destDir "Entities"
    # if (!(Test-Path $entityDestDir)) {
    #     New-Item -Path $entityDestDir -ItemType Directory -Force | Out-Null
    # }
    # # get entity names by cs file name without extension
    # Move-Item -Path $entityPath\* -Destination $entityDestDir -Force

    # remove module reference project
    $moduleProjectFile = Join-Path $solutionPath "src/Modules/"$moduleName "$moduleName.csproj"
    $apiProjectFile = Join-Path $solutionPath "src/Http.API/Http.API.csproj"

    dotnet remove $apiProjectFile reference $moduleProjectFile
    # dotnet sln $solutionPath/MyProjectName.slnx remove $moduleProjectFile
}

# 复原模块内容
function RestoreModule ([string]$solutionPath, [string]$moduleName) {
    Write-Host "restore module:"$moduleName
    $tmp = Join-Path $solutionPath "./.tmp"
    if ((Test-Path $tmp)) {
        # restore module  entity and application from tmp
        $destDir = Join-Path $tmp $moduleName
        $entityDestDir = Join-Path $destDir "Entities"
        $entityPath = Join-Path $solutionPath "./src/Definition/Entity" $moduleName

        Move-Item -Path $entityDestDir\* -Destination $entityPath -Force
    }
    # recover module reference project
    $moduleProjectFile = Join-Path $solutionPath "src/Modules/"$moduleName "$moduleName.csproj"
    dotnet sln $solutionPath/MyProjectName.slnx add $moduleProjectFile
}


$OutputEncoding = [Console]::OutputEncoding = [Text.UTF8Encoding]::UTF8

$location = Get-Location
$entityPath = Join-Path $location "../src/Template/templates/ApiStandard/src/Entity"
$solutionPath = Join-Path $location "../src/Template/templates/ApiStandard"
$tmp = Join-Path $solutionPath "./.tmp"

Write-Host "Clean files"
# delete files

$migrationPath = Join-Path $solutionPath "../src/Template/templates/ApiStandard/src/Http.API/Migrations"
if (Test-Path $migrationPath) {
    Remove-Item $migrationPath -Force -Recurse
}

if (!(Test-Path $tmp)) {
    New-Item -Path $tmp -ItemType Directory -Force | Out-Null
}

try {
    # backup files
    $apiProjectFile = Join-Path $solutionPath "src/Http.API/Http.API.csproj"
    Copy-Item -Path $apiProjectFile -Destination $tmp -Force



    Write-Host "find modules:"$modulesNames;

    foreach ($moduleName in $modulesNames) {
        TempModule $solutionPath $moduleName
    }

    Set-Location ../src/Template
    # pack
    dotnet pack -c release -o ./nuget

    # foreach ($moduleName in $modulesNames) {
    #     RestoreModule $solutionPath $moduleName
    # }

    # restore  files
    Copy-Item -Path $tmp\Http.API.csproj -Destination $solutionPath\src\Http.API -Force

    # delete tmp directory
    Remove-Item $tmp -Force -Recurse

    # get package info
    $VersionNode = Select-Xml -Path ./Pack.csproj -XPath '/Project//PropertyGroup/PackageVersion'
    $PackageNode = Select-Xml -Path ./Pack.csproj -XPath '/Project//PropertyGroup/PackageId'
    $Version = $VersionNode.Node.InnerText
    $PackageId = $PackageNode.Node.InnerText

    #re install package
    dotnet new uninstall $PackageId
    dotnet new install .\nuget\$PackageId.$Version.nupkg
    Set-Location $location;
}
catch {
    Write-Error $_.Exception.Message
    Remove-Item $tmp -Force -Recurse
    Set-Location $location;
}


    
