# bsdk
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe srv.exe
input:*.cer [output:{folder}] batch:serialize

.\kit input:C:\TFS\icao\repo\*.crl "output:{oledb://Data Source=DEV-SR-SQL02\SQL2008SRV;Initial Catalog=icao;User ID=sa;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;Password=escort}" batch:serialize
.\kit input:d:\icao\repo\*.p7b "output:{oledb://Data Source=DEV-SR-SQL02\SQL2008SRV;Initial Catalog=icao;User ID=sa;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;Password=escort}" batch:serialize
.\kit input:c:\tfs\icao\repo\*.cer batch:install storelocation:LocalMachine storename:Root
input:c:\icao\20220426.hexcsv.7z output:c:\icao\rfid batch:extract,group=1 quarantine:c:\failed