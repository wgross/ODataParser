<#
.SYNOPSIS
    Restores the specified dotnet core projects.
    Additionally the script removes the dependency snapshot of the projects (project.lock.json) and the 
    local packages caches can be cleard before restoring.

.EXAMPLE 
    .\restore.ps1 

    builds the projects specified in restore.config or restore.$env:USERNAME.config to the local package directory

.EXAMPLE 
    .\restore.ps1 -RemoveNugetCache all

    builds the projects specified in restore.config or restore.$env:USERNAME.config to the local package directory.
    Before restore all nuget caches are cleared

.EXAMPLE 
    .\restore.ps1 -RemoveProjectLockJson

    builds the projects specified in restore.config or restore.$env:USERNAME.config to the local package directory.
    Before restore the project.lock.json is removed to trigger a complete restore.
#>
[CmdletBinding()]
param(
    [Parameter(Position=0)]
    [ValidateNotNullOrEmpty()]
    $ConfigSection = "default",

    [Parameter()]
    [array]$Path = $null,

    [Parameter()]
    [ValidateSet("all","http-cache","packages-cache","global-packages","temp")]
    [string]$RemoveNugetCache,

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$RemoveGlobalCacheFilter,

    [Parameter()]
    [ValidateScript({Test-Path -Path $_})]
    $ConfigFile = $null
)
begin {
    Import-Module -Name dev-tools -Verbose:$false

    function configFileContentJson {
        if($script:configFileContentJson) {
            return $script:configFileContentJson
        }
        return ($script:configFileContentJson = Get-CIScriptConfigContent -ConfigFilePrefix "restore" -ConfigSection $ConfigSection -ConfigFileDirectory $PSScriptRoot -ConfigFilePath $ConfigFile)
    }
}
process {
    
    #region Clean caches before restore

    if($RemoveNugetCache) {
        "Removing Nuget packages caches: 'nuget locals $RemoveNugetCache -clear'" | Write-Verbose

        & (nugetExe) locals $RemoveNugetCache -clear
    }
    
    if(-not($RemoveGlobalCacheFilter)) {
        "Reading RemoveGlobalCacheFilter from config file $ConfigFile" | Write-Verbose
        
        $RemoveGlobalCacheFilter = (configFileContentJson).removeGlobalCacheFilter
    }

    if($RemoveGlobalCacheFilter) {
        "Removing from Nuget packages cache by filter '$RemoveGlobalCacheFilter'" | Write-Verbose
        
        $env:USERPROFILE | Join-Path -ChildPath ".nuget\packages"| Get-ChildItem -Filter $RemoveGlobalCacheFilter -Directory | Select-Object -ExpandProperty Name | Write-Verbose
        $env:USERPROFILE | Join-Path -ChildPath ".nuget\packages"| Get-ChildItem -Filter $RemoveGlobalCacheFilter -Directory | Remove-Item -Force -Recurse
    }       

    #endregion 

    #region Restore packages

    if(-not($Path)) {
        "Reading pathes to restore from config file $ConfigFile" | Write-Verbose

        $Path = (configFileContentJson).path
    
        if(-not($Path) -or($Path.Length -lt 1)) { throw "no path was specified as parameter or from config file" }
   
        Push-Location -Path $PSScriptRoot
        try
        {
            $Path | Get-DotNetProjectItem | Invoke-DotNetRestore | Write-Output
        } finally {
            Pop-Location
        }
    
    } else {
        # a direct specification of pathes in command line are interpreted in context of 
        # the current working directory

        "Starting restore from given path parameters" | Write-Verbose

        $Path | Get-DotNetProjectItem | Invoke-DotNetRestore | Write-Output
    }
    
    #endregion 
}