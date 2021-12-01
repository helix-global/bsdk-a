using namespace System;
using namespace System.IO;
using namespace System.Text;
using namespace System.Xml
using namespace System.Xml.Xsl

param
    (
    [String]$Source,
    [String]$Target,
    [String]$Transform
    )

$CurrentFolder = (Get-Location).Path;
$CurrentFolder = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path
Set-Location -Path $CurrentFolder

Add-Type -Assembly "System.Xml"
Add-Type -Assembly "System.Xml.ReaderWriter"

Function Dispose([IDisposable]$Object)
    {
    If ($Object -ne $null) {
        $Object.Dispose();
        $Object = $null
        }
    }

If (![Path]::IsPathRooted($Source))    { $Source    = [Path]::Combine($CurrentFolder,$Source);    }
If (![Path]::IsPathRooted($Target))    { $Target    = [Path]::Combine($CurrentFolder,$Target);    }
If (![Path]::IsPathRooted($Transform)) { $Transform = [Path]::Combine($CurrentFolder,$Transform); }

[Stream]$SourceStream = [File]::OpenRead($Source);
[Stream]$TargetStream = [File]::OpenWrite($Target);
#[Stream]$SourceStream = $null;
#[Stream]$TargetStream = $null;
[XmlReader]$Reader = $null;
[XmlWriter]$Writer = $null;

Try
    {
    $Xslt = New-Object XslCompiledTransform
    $Xslt.Load($Transform)
    $Reader = [XmlReader]::Create($SourceStream)
    $Settings = New-Object XmlWriterSettings
    $Settings.Indent = $true;
    $Settings.IndentChars = ' ';
    $Settings.OmitXmlDeclaration = $true;
    $Settings.NewLineHandling;
    $Writer = [XmlWriter]::Create($TargetStream, $Settings);
    $Xslt.Transform($Reader,$Writer)
    #$Xslt.Transform($Source,$Target)
    }
Finally
    {
    Dispose($Reader);
    Dispose($Writer);
    Dispose($SourceStream);
    Dispose($TargetStream);
    }
