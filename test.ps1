<#
.SYNOPSIS
    tests (with xunit) the given dotnet core projects.

.EXAMPLE 
    .\test.ps1 

    builds the projects specified in test.config or test.$env:USERNAME.config to the local package directory
#>
[CmdletBinding()]
param(
    [Parameter(Position=0)]
    [ValidateNotNullOrEmpty()]
    $ConfigSection = "default",

    [Parameter()]
    [array]$Path = $null,

    [Parameter()]
    [ValidateScript({Test-Path -Path $_})]
    $ConfigFile = $null
)
begin {
    Import-Module -Name dev-tools -Verbose:$false

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
                    (Join-Path -Path $PSScriptRoot -ChildPath "test.$Env:USERNAME.config")
                    
                    # common
                    (Join-Path -Path $PSScriptRoot -ChildPath "test.config")
                )

                $ConfigFile = $confgFilePathCandidates | Where-Object -FilterScript {
                    if(Test-Path -Path $_) {
                        # pass thru if it exists
                        $_ | Write-Output
                    }
                } | Select-Object -First 1 
            }

            "Reading config file: $ConfigFile section $ConfigSection" | Write-Verbose

            # from the config file, return the 'test' config
            Get-Content -Path $ConfigFile | ConvertFrom-Json | Select-Object -ExpandProperty $ConfigSection
        }
    }

    function configFileContentJson {
        if($script:configFileContentJson) {
            return $script:configFileContentJson
        }
        return ($script:configFileContentJson = Get-ScriptConfig -ConfigFile $ConfigFile -ConfigSection $ConfigSection)
    }
}
process {
    
    if(-not($Path)) {
        "Reading pathes to test from config file $ConfigFile" | Write-Verbose
        
        $Path = (configFileContentJson).path
    
        if(-not($Path) -or($Path.Length -lt 1)) { throw "no path was specified as parameter or from config file" }

        Push-Location -Path $PSScriptRoot
        try {
            $script:projects = $Path | Get-DotNetProjectItem
        } finally {
            Pop-Location
        }
    
        $script:projects | ForEach-Object -Process {
            Push-Location -Path $_.Directory
            try {
                $_ | Invoke-DotNetTest | Get-XunitTestSummary | Write-Output
            } finally {
                Pop-Location
            }
        }        
    } else {
        # a direct specification of pathes in command line are interpreted in context of 
        # the current working directory
        
        "Starting test from given path parameters" | Write-Verbose
        
        $Path | Get-DotNetProjectItem | Invoke-DotNetTest | Get-XunitTestSummary | Write-Output
    }
}