<#
.SYNOPSIS
    Invokes dotnet run for all projects specified of found in the config file. 
    Because is creates split screen within the console it requires a ConEmu compatible console host
    and an existing Function:\split
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
    
    if(-not(Test-Path -Path Function:\split)) { throw "This script requires a ConEmu host and a PS 'split' function" } 

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
                    (Join-Path -Path $PSScriptRoot -ChildPath "run.$Env:USERNAME.config")
                    
                    # common
                    (Join-Path -Path $PSScriptRoot -ChildPath "run.config")
                )

                $ConfigFile = $confgFilePathCandidates | Where-Object -FilterScript {
                    if(Test-Path -Path $_) {
                        # pass thru if it exists
                        $_ | Write-Output
                    }
                } | Select-Object -First 1 
            }

            "Reading config file: $ConfigFile section $ConfigSection" | Write-Verbose

            # from the config file, return the 'run' config
            Get-Content -Path $ConfigFile | ConvertFrom-Json | Select-Object -ExpandProperty $ConfigSection
        }
    }

    function configFileContentJson {
        if($script:configFileContentJson) {
            return $script:configFileContentJson
        }
        return ($script:configFileContentJson = Get-ScriptConfig -ConfigFile $ConfigFile -ConfigSection $ConfigSection)
    }

    function Invoke-DotNetRun {
        [CmdletBinding()]
        param(
            [Parameter(ValueFromPipeline,Mandatory,HelpMessage="give a path to a dotnet core compatible .csproj file")]
            [ValidateScript({$_ |Test-Path -PathType Leaf})]
            $Project = $null
        )
        process {
            $projectItem = $Project | Get-Item

            "Invoking dotNet run $($projectItem.FullName)" | Write-Verbose

            Push-Location $projectItem.Directory
            try {

                split -Command "dotNet run $($projectItem.FullName)" -Vertical -Title "$($projectItem.BaseName)"

            } finally {
                Pop-Location
            }
        }
    }

    function Get-DotNetProjectItem {
        [CmdletBinding()]
        param(
            [Parameter(ValueFromPipeline)]
            [ValidateScript({Test-Path -Path $_})]
            $Path = $PWD,
        
            [Parameter()]
            [ValidateNotNull()]
            [Scriptblock]$Process = $null,

            [Parameter(DontShow)]
            [ValidateNotNullOrEmpty()]
            [string]$Filter = "*.csproj"
        )
        process {
     
            # by combining Get-Item -Path | Get-ChildItem -Filter a path filter from $Projects and a 
            # hard coded filter for child items *.csproj are combined

            if($Process) {
                Get-item -Path $Path | Get-ChildItem -Filter $Filter -Recurse -File | ForEach-Object -Process $Process
            } else {
                Get-item -Path $Path | Get-ChildItem -Filter $Filter -Recurse -File | Write-Output
            }
        }
    }
}
process {
    
    if(-not($Path)) {
        # read projects list from config file.
        $Path = (configFileContentJson).path
    }

    if(-not($Path) -or($Path.Length -lt 1)) { throw "no path was specified as parameter or from config file" }

    $PSScriptRoot | Push-Location 
    try {
        $Path | Get-DotNetProjectItem | Invoke-DotNetRun
    } finally {
        Pop-Location
    }
}