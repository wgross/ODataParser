using namespace System.Security.Principal

<#
.SYNOPSIS
    Builds the the given dotnet core projects. The projects to publis are read fro a config file.
    The configfile determines the destination ditrectory as well as the web application name

.EXAMPLE 
    .\publish.ps1 

    publishes the projects specified in publish.config or publish.$env:USERNAME.config 
#>
[CmdletBinding()]
param(
    [Parameter(Position=0)]
    [ValidateNotNullOrEmpty()]
    $ConfigSection = "default",

    [Parameter()]
    [ValidateScript({$_ | Test-Path -PathType Leaf})]
    $ConfigFile = $null
)
begin {
    Import-Module -Name dev-tools -Verbose:$false
    Import-Module -Name WebAdministration -Verbose:$false

    function sinInt5NetCoreAppPool {
        if(-not(Test-Path -Path IIS:\AppPools\SinInt5.NetCore)) {
            "Creating app pool 'SinInt5.NetCore'" | Write-Verbose

            $appPool = New-WebAppPool -Name "SinInt5.NetCore" 
            $appPool | Set-ItemProperty -Name processModel.identityType -Value 2 # 2-NetworkService
            $appPool | Set-ItemProperty -Name managedRuntimeVersion -Value ([string]::Empty)
        }

        return Get-Item -Path IIS:\AppPools\SinInt5.NetCore 
    }

    function isAdmin {
        return [WindowsPrincipal]::new([WindowsIdentity]::GetCurrent()).IsInRole([WindowsBuiltInRole]::Administrator)
    }

    function Get-ScriptConfig {
        param(
            [Parameter()]
            $ConfigFile,

            [Parameter()]
            [ValidateNotNullOrEmpty()]
            [string]$ConfigSection
        )
        process {
            if(-not($ConfigFile)) {
                
                # no config file given, make a sophisticated guess

                $confgFilePathCandidates = @( 
                    
                    # user specific
                    (Join-Path -Path $PSScriptRoot -ChildPath "publish.$Env:USERNAME.config")
                    
                    # common
                    (Join-Path -Path $PSScriptRoot -ChildPath "publish.config")
                )

                $ConfigFile = $confgFilePathCandidates | Where-Object -FilterScript {
                    if(Test-Path -Path $_) {
                        # pass thru if it exists
                        $_ | Write-Output
                    }
                } | Select-Object -First 1 
            }

            "Reading config file: $ConfigFile section $ConfigSection" | Write-Verbose

            # from the config file, return the 'build' config
            Get-Content -Path $ConfigFile | ConvertFrom-Json | Select-Object -ExpandProperty $ConfigSection
        }
    }

    function configFileContentJson {
        if($script:configFileContentJson) {
            return $script:configFileContentJson
        }
        return ($script:configFileContentJson = Get-ScriptConfig -ConfigFile $ConfigFile -ConfigSection $ConfigSection)
    }

    function New-SinIntWebApplication {
        param(
            [Parameter(ValueFromPipelineByPropertyName)]
            [Alias("source")] # name in in confg file
            [ValidateScript({Test-Path -Path $_})]
            $Project,

            [Parameter(ValueFromPipelineByPropertyName)]
            [ValidateScript({Test-Path -Path $_})]
            $Destination,

            [Parameter(ValueFromPipelineByPropertyName)]
            [ValidateNotNullOrEmpty()]
            $Name    
        )
        begin {
            $defaultWebSite = (Get-Website | Select-Object -First 1)            
        }
        process {
            $sourceItem = Get-Item -Path $Project
            $destinationItem = Get-Item -Path $Destination

            "Creating web application $Name in site $($defaultWebSite.Name) at $($destinationItem.FullName)" | Write-Verbose

            New-WebApplication -Site $defaultWebSite.Name -Name $Name -ApplicationPool (sinInt5NetCoreAppPool).Name -PhysicalPath $destinationItem.FullName -Force
        
            "Creating pseudo drive name '$Name`:" | Write-Verbose

            Invoke-Expression -Command "function global:$Name`: { Set-Location -Path $($destinationItem.FullName) }"
        }
    }

    function Invoke-ConfigureScript {
        param(
            [Parameter(ValueFromPipelineByPropertyName)]
            [ValidateScript({$_ | Test-Path -PathType Container})]
            $Destination
        )
        process {
            $destinationItem = $Destination | Get-Item 
            $configurePath = (Join-Path -Path $destinationItem  -ChildPath "configure.ps1")
            if($configurePath | Test-Path -PathType Leaf) {
                "Calling configure script $configurePath" | Write-Verbose
                & $configurePath
            }
        }
    }

    "Resetting IIS" | Write-Verbose
    iisreset | Write-Host
}
process {
    if(-not(isAdmin)) { throw "User $Env:UserName must be admin to publish" }

    # read projects list from config file.
    $Projects = (configFileContentJson).projects

    if(-not($Projects) -or($Projects.Length -lt 1)) { throw "no project was specified in config file" }
    
    Push-Location $PSScriptRoot # interprete relative pathes from config files relative to the script root not the $PWD
    try {
        $publishedProjects = $Projects | Invoke-DotNetPublish 
        $publishedProjects | Where-Object -FilterScript { $_.Succeeded } | Invoke-ConfigureScript
        $publishedProjects | Write-Output
        $Projects | New-SinIntWebApplication | Out-Null
    } finally {
        Pop-Location
    }
}
end {
    "Resetting IIS" | Write-Verbose
    iisreset | Write-Host
}