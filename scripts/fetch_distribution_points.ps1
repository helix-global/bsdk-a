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

$Version = [System.Reflection.Assembly]::GetExecutingAssembly().ImageRuntimeVersion
Write-Host $Version -ForegroundColor Yellow

Try
    {
    #[System.AppDomain]::CurrentDomain.Add_AssemblyLoad({
    #    param($Sender,[System.AssemblyLoadEventArgs]$Args)
    #    $Name = $Args.LoadedAssembly;
    #    Write-Host "Loading Assembly {$Name}" -ForegroundColor Yellow
    #    });
    #[System.AppDomain]::CurrentDomain.Add_AssemblyResolve({
    #    param($Sender,[System.ResolveEventArgs]$Args)
    #    Write-Host "Resolving Assembly {$Args.Name}" -ForegroundColor Yellow
    #    });
    #[System.AppDomain]::CurrentDomain.Add_TypeResolve({
    #    param($Sender,[System.ResolveEventArgs]$Args)
    #    Write-Host "Resolving Type {$Args.Name}" -ForegroundColor Yellow
    #    });
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
$TargetFolder = [Path]::Combine($CurrentFolder,".\..\");
$Items  = New-Object HashSet[String]
$URIs   = New-Object HashSet[String]
$SuccessURIs = New-Object HashSet[String]

$UseBlindLoading = $false
$UseLoading = $false

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
                        ForEach($E In $Target.Extensions) {
                            If ($E -is [CRLDistributionPoints]) {
                                ForEach($DistributionPoint In ($E -as [CRLDistributionPoints]).DistributionPoints) {
                                    ForEach($FullName In $DistributionPoint.Point.FullName) {
                                        $Key = $FullName.ToString()
                                        If ($Key.StartsWith("http")) {
                                            $Status = $URIs.Add($Key)
                                            }
                                        $Key = [String]::Format("{{{0}}}:{{{1}}}",$Target.Country,$Key, $FileItem)
                                        If ($Items.Add($Key)) {
                                            Write-Output $Key
                                            }
                                        }
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

Function Load-URI([String]$URI) {
    $HttpUri = New-Object System.Uri @($URI,[UriKind]::Absolute)
    $HttpClientHandler = New-Object System.Net.Http.HttpClientHandler
    $Client = New-Object HttpClient -ArgumentList @(,$HttpClientHandler)
    Try
        {
        $ClientResult = $Client.GetStreamAsync($HttpUri).GetAwaiter().GetResult();
        $FileName = [Path]::GetFileName($HttpUri.AbsolutePath)
        $TargetStream = New-Object MemoryStream
        Try
            {
            $ClientResult.CopyTo($TargetStream)
            $SourceStream = New-Object ReadOnlyMemoryMappingStream -ArgumentList @(,$TargetStream.ToArray())
            Try
                {
                $Source = [Enumerable]::FirstOrDefault([Asn1Object]::Load($SourceStream));
                If ($Source -ne $null) {
                    $Target = New-Object Asn1CertificateRevocationList -ArgumentList @(,$Source);
                    if (!$Target.IsFailed) {
                        $LocalTargetFolder = $TargetFolder
                        $Country = $Target.Country
                        If ($Country -ne $null) {
                            $LocalTargetFolder = [Path]::Combine($TargetFolder,$Country)
                            if (!(Test-Path -Path $LocalTargetFolder)) {
                                New-Item -ItemType Directory -Force -Path $LocalTargetFolder
                                }
                            }
                        $TargetFileName = [Path]::Combine($LocalTargetFolder,$Target.FriendlyName + ".crl");
                        [File]::WriteAllBytes($TargetFileName, $TargetStream.ToArray())
                        If ($SuccessURIs.Add($URI))
                            {
                            Write-Host "{$URI}:{$TargetFileName}" -ForegroundColor Green
                            }
                        }
                    }
                }
            Finally
                {
                $SourceStream.Dispose()
                }
            }
        Finally
            {
            $TargetStream.Dispose()
            }
        }
    Catch
        {
        $Message = $_.Exception.Message
        Write-Host "{$URI}:{$Message}" -ForegroundColor Red
        }
    Finally
        {
        $Client.Dispose()
        }
    }

Function Load-URIS([HashSet[String]]$URIs) {
    ForEach($URI In $URIs) {
        Load-URI($URI)
        }
    }

[ServicePointManager]::Expect100Continue = $true
[ServicePointManager]::SecurityProtocol =
    [SecurityProtocolType]::Tls   +
    [SecurityProtocolType]::Tls11 +
    [SecurityProtocolType]::Tls12 +
    [SecurityProtocolType]::Ssl3
[ServicePointManager]::CertificatePolicy = [BinaryStudio.DirectoryServices.Utilities]::IgnoreCertificatePolicy

If($UseBlindLoading) {
    Write-Output "------------ BLIND LOADING ----------"
    ForEach($Key In [BinaryStudio.DirectoryServices.Country]::Countries.Keys) {
        $Country = [BinaryStudio.DirectoryServices.Country]::Countries[$Key]
        $CountryLong = $Country.ThreeLetterISOCountryName.ToUpper()
        Load-URI([String]::Format("https://pkddownload1.icao.int/CRLs/{0}.crl", $CountryLong))
        Load-URI([String]::Format("https://pkddownload2.icao.int/CRLs/{0}.crl", $CountryLong))
        }
    }
If($UseLoading) {
    Write-Output "------------ LOADING ----------"
    Load-URIS($URIs)
    }
Write-Output "------------ SUCCESS ----------"
ForEach($Key In $SuccessURIs) {
    Write-Host $Key -ForegroundColor Yellow
    }