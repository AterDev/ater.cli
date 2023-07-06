[CmdletBinding()]
param (
    [Parameter()]
    [System.Boolean]
    $withStudio = $false
)
$location = Get-Location
$OutputEncoding = [System.Console]::OutputEncoding = [System.Console]::InputEncoding = [System.Text.Encoding]::UTF8
try {

    Set-Location $location
    # get package name and version
    $VersionNode = Select-Xml -Path ../src/CommandLine/CommandLine.csproj -XPath '/Project//PropertyGroup/Version'
    $PackageNode = Select-Xml -Path ../src/CommandLine/CommandLine.csproj -XPath '/Project//PropertyGroup/PackageId'
    $Version = $VersionNode.Node.InnerText
    $PackageId = $PackageNode.Node.InnerText


    # sync studio version
    Set-Location $location
    $xml = [xml](Get-Content ../src/AterStudio/AterStudio.csproj)
    $propertyGroup = $xml.Project.PropertyGroup[0]
    Write-Host $Version
    if ($null -eq $propertyGroup.Version) {
        $version = $xml.CreateElement("Version")
        
        $version.InnerText = "$Version"
        $propertyGroup.AppendChild($version)
    }
    else {
        $propertyGroup.Version = "$Version"
    }
    $path = Join-Path  $location "../src/AterStudio/AterStudio.csproj"
    $xml.Save($path)


    if ($withStudio -eq $true) {
        # build web project
        Set-Location ../src/AterStudio
        if (Test-Path -Path ".\publish") {
            Remove-Item .\publish -R -Force
        }
        
        dotnet publish -c release -o ./publish
        # 移除部分 dll文件，减少体积
        Remove-Item .\publish\Microsoft.CodeAnalysis.CSharp.dll
        Remove-Item .\publish\Swashbuckle.AspNetCore.SwaggerUI.dll
        Remove-Item .\publish\Microsoft.CodeAnalysis.dll
        Remove-Item .\publish\LiteDB.dll
        Remove-Item .\publish\Microsoft.OpenApi.Readers.dll
        Remove-Item .\publish\Microsoft.OpenApi.dll
        Remove-Item .\publish\SharpYaml.dll
        Remove-Item .\publish\AterStudio.exe

        Remove-Item .\publish\CodeGenerator.dll
        Remove-Item .\publish\Command.Share.dll
        Remove-Item .\publish\Core.dll
        Remove-Item .\publish\Datastore.dll
        Remove-Item .\publish\swagger.json
        Compress-Archive -Path .\publish\*  -DestinationPath "../CommandLine/studio.zip" -CompressionLevel Optimal -Force
    }

    Set-Location $location
    Set-Location ../src/CommandLine
    Write-Host 'build and pack new version...'
    # build & pack
    dotnet build -c release
    dotnet pack --no-build -c release
    
    # uninstall old version
    Write-Host 'uninstall old version'
    dotnet tool uninstall -g $PackageId

    Write-Host 'install new version'
    dotnet tool install -g --add-source ./nupkg $PackageId --version $Version

    Set-Location $location
}
catch {
    Write-Host $_.Exception.Message
}
