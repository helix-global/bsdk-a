-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ReportG1]
AS
BEGIN
  SET NOCOUNT ON;
  CREATE TABLE #Country
    (
    [Country] VARCHAR(5) NOT NULL
    )

  INSERT INTO #Country ([Country])
  SELECT DISTINCT [a].[Country] FROM [dbo].[Certificate] [a] ORDER BY 1

  SELECT
     [a].[Country]
    ,[a].[ObjectId] [SubjectCertificate]
    ,(CAST([dbo].[StringEquals](ISNULL([a].[AuthorityKeyIssuer],[b].[SubjectKeyIdentifier]) ,[a].[SubjectKeyIdentifier],'OrdinalIgnoreCase') AS INT) +
      CAST([dbo].[StringEquals]([a].[Issuer],[a].[Subject],'OrdinalIgnoreCase') AS INT)) [IsRoot]
    ,ISNULL([a].[AuthorityKeyIssuer],[b].[SubjectKeyIdentifier]) [AuthorityKeyIssuer]
    ,[a].[SubjectKeyIdentifier]
  INTO #Certificate
  FROM [dbo].[ReportCertificate] [a]
    LEFT JOIN [dbo].[ReportCertificate] [b] ON ([b].[Country]=[a].[Country]) AND ([a].[IssuerSerialNumber]=[b].[SerialNumber])
  WHERE ([a].[Country]<>'ru')
     OR ([a].[Country]='ru') AND (NOT [a].[Issuer] LIKE '%восход%')

    --SELECT * FROM #Certificate WHERE [SubjectCertificate]=57147

  SELECT
     [a].[Country]
    ,[a].[ObjectId] [SubjectCertificate]
    ,[b].[ObjectId] [IssuerCertificate]
  INTO #CertificateChainInfo
  FROM [dbo].[ReportCertificate] [a]
    LEFT JOIN [dbo].[ReportCertificate] [b] ON ([b].[Country]=[a].[Country]) AND ([b].[Subject]=[a].[Issuer])
        AND (([a].[IssuerSerialNumber] IS NULL) OR ([a].[IssuerSerialNumber]=[b].[SerialNumber]))
        AND ([a].[AuthorityKeyIdentifier]=[b].[SubjectKeyIdentifier])
        AND ([a].[AuthorityKeyIdentifier]<>[a].[SubjectKeyIdentifier])
  WHERE ([a].[Country]<>'ru')
     OR ([a].[Country]='ru') AND (NOT [a].[Issuer] LIKE '%восход%')
  ORDER BY [a].[Country]

  IF OBJECT_ID('tempdb..##CertificateReport') IS NOT NULL DROP TABLE ##CertificateReport
  CREATE TABLE ##CertificateReport
  (
    [Base] INT NULL,
    [Country] VARCHAR(5) COLLATE Cyrillic_General_CI_AS NULL,
    [IsRoot] INT NULL,
    [Level] INT NULL,
    [Children] INT NULL,
    [SerialNumber] NVARCHAR(MAX) NULL,
    [Issuer] NVARCHAR(MAX) NULL,
    [Subject] NVARCHAR(MAX) NULL,
    [SubjectKeyIdentifier] NVARCHAR(MAX) NULL,
    [AuthorityKeyIdentifier] NVARCHAR(MAX) NULL,
    [Thumbprint] NVARCHAR(MAX) NULL,
    [SignatureAlgorithm] NVARCHAR(MAX) NULL,
    [HashAlgorithm] NVARCHAR(MAX) NULL,
    [NotBefore] DATETIME NULL,
    [NotAfter] DATETIME NULL
  )

  ;WITH [T] AS
  (
    SELECT
       [a].[SubjectCertificate]
      ,(SELECT COUNT(*) FROM #CertificateChainInfo [b] WHERE ([b].[SubjectCertificate]=[a].[SubjectCertificate]) AND ([b].[IssuerCertificate] IS NOT NULL)) [Children]
    FROM #Certificate [a]
      INNER JOIN [dbo].[ReportCertificate] [c] ON [c].[ObjectId]=[a].[SubjectCertificate]
    WHERE [c].[NotAfter]>=GETDATE()
  )
  INSERT INTO ##CertificateReport
  SELECT
     [b].[SubjectCertificate] [Base]
    ,[b].[Country]
    ,[b].[IsRoot]
    ,0 [Level]
    ,[a].[Children]
    ,[c].[SerialNumber]
    ,[c].[Issuer]
    ,[c].[Subject]
    ,[c].[SubjectKeyIdentifier]
    ,[c].[AuthorityKeyIdentifier]
    ,[c].[Thumbprint]
    ,[c].[SignatureAlgorithm]
    ,[c].[HashAlgorithm]
    ,[c].[NotBefore]
    ,[c].[NotAfter]
  FROM [T] [a]
    INNER JOIN #Certificate [b] ON [b].[SubjectCertificate]=[a].[SubjectCertificate]
    INNER JOIN [dbo].[ReportCertificate] [c] ON [c].[ObjectId]=[a].[SubjectCertificate]

  INSERT INTO ##CertificateReport
  SELECT
     [a].[SubjectCertificate] [Base]
    ,[a].[Country]
    ,NULL [IsRoot]
    ,1 [Level]
    ,NULL [Children]
    ,[c].[SerialNumber]
    ,[c].[Issuer]
    ,[c].[Subject]
    ,[c].[SubjectKeyIdentifier]
    ,[c].[AuthorityKeyIdentifier]
    ,[c].[Thumbprint]
    ,[c].[SignatureAlgorithm]
    ,[c].[HashAlgorithm]
    ,[c].[NotBefore]
    ,[c].[NotAfter]
  FROM #CertificateChainInfo [a]
    INNER JOIN [dbo].[ReportCertificate] [b] ON [b].[ObjectId]=[a].[SubjectCertificate]
    LEFT  JOIN [dbo].[ReportCertificate] [c] ON [c].[ObjectId]=[a].[IssuerCertificate]
  WHERE [b].[NotAfter]>=GETDATE()

  SELECT
    [b].[RussianShortName],
    [a].*,
    CASE ISNULL([a].[IsRoot],-1)
      WHEN 0 THEN
        CASE [a].[Level]
          WHEN 0 THEN
            CASE [dbo].[Int32GT]([a].[Children],0)
              WHEN 1 THEN 'False'
              ELSE 'True'
            END
          ELSE NULL
        END
      WHEN -1 THEN NULL
      ELSE 'False'
    END [IsError]
  FROM ##CertificateReport [a]
    --INNER JOIN [dbo].[Country] [b] ON [dbo].[StringEquals]([b].[Country],[a].[Country],'OrdinalIgnoreCase') = 1
    INNER JOIN [dbo].[Country] [b] ON [b].[Country]=[a].[Country]
  ORDER BY [a].[Country]
    ,[a].[Base]
    ,[a].[Level]
    ,LEN([a].[SerialNumber])
    ,[a].[SerialNumber]
    ,[a].[IsRoot],[a].[Children]

  DROP TABLE #Country
  DROP TABLE #CertificateChainInfo
  DROP TABLE #Certificate
END