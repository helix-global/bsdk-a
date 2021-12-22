using namespace System.Diagnostics
using namespace System.IO
using namespace System.Linq
using namespace BinaryStudio.IO
using namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

param
    (
    [String]$InputFileName="���������25.11.21.bin",
    [String]$OutputFolder=".output",
    [Int32]$StartIndex=0
    )

Add-Type -Assembly "System.Data"
Add-Type -Assembly "System"

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
Write-Host $InputFileName -ForegroundColor Yellow

Try
    {
    Internal-Add-Type "BinaryStudio.Security.Cryptography.dll"
    Internal-Add-Type "BinaryStudio.IO.dll"
    Internal-Add-Type "BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.dll"
    Internal-Add-Type "BinaryStudio.Security.Cryptography.Certificates.dll"
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

Function Write-Info([Int64]$Offset,[String]$Message,$ForegroundColor)
    {
    $Output = [String]::Format("{{{0}}}:",$Offset.ToString("X8"))
    $Output = $Output + $Message
    Write-Host $Output -ForegroundColor $ForegroundColor
    }

Function Dispose([IDisposable]$Object)
    {
    If ($Object -ne $null) {
        $Object.Dispose();
        $Object = $null
        }
    }

Function ByPass($Condition) {
    If ($Condition -eq $null) {
        }
    }

Function Read([Stream]$Stream)
    {
    Return $Stream.ReadByte()
    }

Function Peek([Stream]$Stream)
    {
    $P = $Stream.Position
    $Value = Read($Stream)
    $Stream.Position = $P
    Return $Value
    }

[Stream]$InputFile=$null
[StreamReader]$InputFileReader=$null
Try
    {
    $CurrentFolder   = (Get-Location).Path;
    $CurrentFolder   = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path
    $OutputFolder    = [Path]::Combine($CurrentFolder,$OutputFolder)
    If (![Directory]::Exists($OutputFolder))
        {
        [Directory]::CreateDirectory($OutputFolder)
        }
    $InputFile       = New-Object ReadOnlyFileMappingStream @(,[Path]::Combine($CurrentFolder,$InputFileName))
    $Buffer = New-Object Byte[] 8192
    While ($true) {
        ByPass($InputFile.Seek(64, [SeekOrigin]::Current))
        ByPass($InputFile.Seek( 2, [SeekOrigin]::Current))
        [String]$FileId = ""
        $FileIdChar = 0
        While ($true) {
            $FileIdChar = Peek($InputFile)
            If ($FileIdChar -eq 13) {
                Break;
                }
            ByPass(Read($InputFile))
            $FileId = $FileId + $FileIdChar.ToChar($null)
            }
        ByPass($InputFile.Seek( 2, [SeekOrigin]::Current))
        ByPass($InputFile.Seek(64, [SeekOrigin]::Current))
        ByPass($InputFile.Seek( 2, [SeekOrigin]::Current))
        ByPass($InputFile.Seek(64, [SeekOrigin]::Current))
        #[Console]::WriteLine([String]::Format("{{{0}}}:{1}",
        #    $InputFile.Position.ToString("D8"),
        #    (Peek($InputFile)).ToString("X2")))
        $I = 0
        $J = $InputFile.Position
        #Write-Host "$FileId" -ForegroundColor Red
        ForEach($O In [Asn1Object]::Load($InputFile,0)) {
            [Stream]$OutputFile=$null
            [Stream]$InputObjectFile=$null
            [Stream]$OutputCertificateFile=$null
            $I = $I + 1
            $P = $InputFile.Position
            $OutputFileName = [String]::Format("efsod{0}+{1}.hex",$StartIndex.ToString("D4"),$FileId)
            Try
                {
                $OutputFile=[File]::OpenWrite([Path]::Combine($OutputFolder,$OutputFileName))
                $O.Write($OutputFile)
                Write-Info -Offset $J -Message $OutputFileName -ForegroundColor Gray
                $StartIndex = $StartIndex + 1
                }
            Finally
                {
                Dispose($OutputFile)
                }
            Try
                {
                $InputObjectFile=[File]::OpenWrite([Path]::Combine($OutputFolder,$OutputFileName))
                }
            Finally
                {
                }
            Try
                {
                $InputFile.Position = $P
                }
            Catch
                {
                }
            }
        If ($I -eq 0)
            {
            Break;
            }
        #Break;
        }
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
Finally
    {
    Dispose($InputFile)
    }