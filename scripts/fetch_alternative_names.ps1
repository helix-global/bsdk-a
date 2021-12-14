using namespace System.Linq
using namespace System.Collections.Generic; 
using namespace System.Net.Http;
using namespace System.Net;
using namespace System.IO;
using namespace BinaryStudio.IO 
using namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
using namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions

param
(
[String]$SelectedCountry = "*"
)

$CurrentFolder = (Get-Location).Path;
$CurrentFolder = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path
Set-Location -Path $CurrentFolder

Function Internal-Add-Type([String]$Path)
    {
    Try
        {
        Add-Type -Path $Path
        }
    Catch [Exception]
        {
        $_.Exception.Data["Path"] = $Path;
        Throw
        }
    }

Function Add-Value([HashSet[String]]$Source,[String]$Value) {
    If($Source.Add($Value)) {
        }
    Else
        {
        }
    }

$Version = [System.Reflection.Assembly]::GetExecutingAssembly().ImageRuntimeVersion
Write-Host $Version -ForegroundColor Yellow

Try
    {
    Internal-Add-Type "BinaryStudio.Security.Cryptography.dll"
    Internal-Add-Type "BinaryStudio.IO.dll"
    Internal-Add-Type "BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.dll"
    Internal-Add-Type "BinaryStudio.Security.Cryptography.Certificates.dll"
    Internal-Add-Type "BinaryStudio.DirectoryServices.dll"
    Add-Type -Assembly "System.Net.Http"
    Add-Type -Assembly "System.Net"
    }
Catch [Exception]
    {
    Write-Host $_.Exception.ToString() -ForegroundColor Red
    $I = 0
    ForEach ($Key In $_.Exception.Data.Keys) {
        If ($I -eq 0) {
            Write-Host "Exception Data:" -ForegroundColor Red
            }
        $Value = $_.Exception.Data[$Key];
        Write-Host "    {$Key}:{$Value}" -ForegroundColor Red
        }
    Exit
    }


$Folder = [Path]::Combine($CurrentFolder,".\..\");
If(($SelectedCountry -ne "*") -and ($SelectedCountry -ne "")) {
    $Folder = [Path]::Combine($CurrentFolder,[Path]::Combine(".\..\",$SelectedCountry))
    }

$SbjAltNames = New-Object HashSet[String]
$IsrAltNames = New-Object HashSet[String]
$SbjAltToCertAssoc = [ordered]@{}
$IsrAltToCertAssoc = [ordered]@{}

Write-Host "Collecting..." -ForegroundColor Yellow
ForEach ($FileItem in [System.IO.Directory]::EnumerateFiles($Folder,"*.*", [System.IO.SearchOption]::AllDirectories)) {
    $Ext = [System.IO.Path]::GetExtension($FileItem).ToLower();
    Switch($Ext)
        {
        ".cer"
            {
            $Bytes = [File]::ReadAllBytes($FileItem);
            $Stream = New-Object ReadOnlyMemoryMappingStream -ArgumentList @(,$Bytes)
            Try
                {
                $Source = [Enumerable]::FirstOrDefault([Asn1Object]::Load($Stream));
                if ($Source -ne $null) {
                    $Target = New-Object Asn1Certificate -ArgumentList @(,$Source);
                    if (!$Target.IsFailed) {
                        [String]$Certificate = [String]::Format("{{{0}}}:{{{1}}}",$Target.Country,$Target.FriendlyName)
                        ForEach($E In $Target.Extensions) {
                            If ($E -is [Asn1SubjectAlternativeName]) {
                                $Key = $E.ToString()
                                [HashSet[String]]$Values = $null
                                If($SbjAltToCertAssoc.Contains($Key)) {
                                    $Values = $SbjAltToCertAssoc[$Key]
                                    }
                                Else
                                    {
                                    $Values = New-Object HashSet[String]
                                    $SbjAltToCertAssoc[$Key] = $Values
                                    }
                                Add-Value -Source $Values -Value $Certificate
                                If ($SbjAltNames.Add($Key)) {
                                    $Key = [String]::Format("{{sbj}}:{{{0}}}:{{{1}}}",$Target.Country,$Key, $FileItem)
                                    Write-Output $Key
                                    }
                                }
                            ElseIf ($E -is [Asn1IssuerAlternativeName]) {
                                $Key = $E.ToString()
                                [HashSet[String]]$Values = $null
                                If($IsrAltToCertAssoc.Contains($Key)) {
                                    $Values = $IsrAltToCertAssoc[$Key]
                                    }
                                Else
                                    {
                                    $Values = New-Object HashSet[String]
                                    $IsrAltToCertAssoc[$Key] = $Values
                                    }
                                Add-Value -Source $Values -Value $Certificate
                                If ($IsrAltNames.Add($Key)) {
                                    $Key = [String]::Format("{{isr}}:{{{0}}}:{{{1}}}",$Target.Country,$Key, $FileItem)
                                    Write-Output $Key
                                    }
                                }
                            }
                        }
                    }
                }
            Finally
                {
                If ($Stream -ne $null) {
                    $Stream.Dispose();
                    $Stream = $null
                    }
                }
            }
        }
    }
Write-Output "[SbjName->SbjName]..."
ForEach($SbjAltName In $SbjAltNames) {
    [HashSet[String]]$Certificates = $SbjAltToCertAssoc[$SbjAltName]
    $Key = [String]::Format("{{sbj}}:{{{0}}}:{{Count={1}}}",$SbjAltName,$Certificates.Count)
    Write-Output $Key
    ForEach($Certificate In $Certificates) {
        Write-Output "  $Certificate"
        }
    }
Write-Output "IsrName->IsrName..."
ForEach($IsrAltName In $IsrAltNames) {
    [HashSet[String]]$Certificates = $IsrAltToCertAssoc[$SbjAltName]
    $Key = [String]::Format("{{isr}}:{{{0}}}:{{Count={1}}}",$IsrAltName,$Certificates.Count)
    Write-Output $Key
    ForEach($Certificate In $Certificates) {
        Write-Output "  $Certificate"
        }
    }

Write-Output "SbjName->IsrName..."
ForEach($SbjAltName In $SbjAltNames) {
    If($IsrAltToCertAssoc.Contains($SbjAltName)) {
        [HashSet[String]]$Certificates = $IsrAltToCertAssoc[$SbjAltName]
        $Key = [String]::Format("{{sbj}}:{{{0}}}:{{Count={1}}}",$SbjAltName,$Certificates.Count)
        Write-Output $Key
        ForEach($Certificate In $Certificates) {
            Write-Output "  $Certificate"
            }
        }
    }

Write-Output "IsrName->SbjName..."
ForEach($IsrAltName In $IsrAltNames) {
    If($SbjAltToCertAssoc.Contains($IsrAltName)) {
        [HashSet[String]]$Certificates = $SbjAltToCertAssoc[$IsrAltName]
        $Key = [String]::Format("{{isr}}:{{{0}}}:{{Count={1}}}",$IsrAltName,$Certificates.Count)
        Write-Output $Key
        ForEach($Certificate In $Certificates) {
            Write-Output "  $Certificate"
            }
        }
    }