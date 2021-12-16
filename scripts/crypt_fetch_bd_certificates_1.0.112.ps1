param
    (
    [String]$ConnectionString = "Data Source=10.0.240.19\sql2012srv;Initial Catalog=pakdb;Persist Security Info=True;User ID=sa;Password=escort"
    )

Add-Type -Assembly "System.Data"

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
Write-Host $ConnectionString -ForegroundColor Yellow

Try
    {
    Internal-Add-Type "FT.Security.Cryptography.dll"
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

Function Dispose([IDisposable]$Object)
    {
    If ($Object -ne $null) {
        $Object.Dispose();
        $Object = $null
        }
    }

[System.Data.SqlClient.SqlConnection]$Connection = $null
[System.Data.SqlClient.SqlCommand]$Command = $null
[System.Data.SqlClient.SqlDataReader]$Reader = $null

Try
    {
    $Connection = New-Object System.Data.SqlClient.SqlConnection
    $Connection.ConnectionString = $ConnectionString
    $Connection.Open()
    $Command = $Connection.CreateCommand()
    $Command.CommandText = "SELECT * FROM [dbo].[UserInfo]"
    $Reader = $Command.ExecuteReader()
    $Store = [FT.Security.Cryptography.SecApi]::OpenCertificateStore(5)
    While ($Reader.Read()) {
        $UserInfoId = $Reader.GetValue($Reader.GetOrdinal("UserInfoID"))
        $CertificateBody = $Reader.GetSqlBinary($Reader.GetOrdinal("CertificateRutoken"))
        If (!$CertificateBody.IsNull) {
            $CertificateBytes = $CertificateBody.Value
            Try
                {
                $Certificate = $Store.LoadCertificateFromBLOB($CertificateBytes)
                $e = $Certificate.Verify()
                If (!$e.IsError) {
                    Write-Host "    {UserInfoID}:{$UserInfoId}:{OK}" -ForegroundColor Green
                    }
                Else
                    {
                    $Message = $e.Message
                    Write-Host "    {UserInfoID}:{$UserInfoId}:{$Message}" -ForegroundColor Red
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
                Write-Host "    {UserInfoID}:{$UserInfoId}" -ForegroundColor Red
                }
            }
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
    Dispose($Command)
    Dispose($Connection)
    }