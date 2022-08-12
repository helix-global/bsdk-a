-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ReportG2]
AS
BEGIN
  CREATE TABLE #Country
    (
    [Country] VARCHAR(5)  COLLATE Cyrillic_General_CI_AS NOT NULL
    )

  INSERT INTO #Country ([Country])
  SELECT DISTINCT [a].[Country] FROM [dbo].[Certificate] [a] ORDER BY 1

  SELECT
     [a].[Country]
    ,[a].[ObjectId] [Subject]
    ,[b].[ObjectId] [Issuer]
  INTO #CrlPairs
  FROM [dbo].[ReportCertificateRevocationList] [a]
    LEFT JOIN [dbo].[ReportCertificate] [b] ON
            ([b].[Country]=[a].[Country])
        AND ([b].[NotAfter]>=GETDATE())
        AND ((([a].[AuthorityKeyIdentifier] IS NOT NULL) AND ([a].[AuthorityKeyIdentifier]=[b].[SubjectKeyIdentifier]) AND ([a].[Issuer]=[b].[Subject]))
           OR (([a].[AuthorityKeyIdentifier] IS NULL) AND ([a].[Issuer]=[b].[Subject])))
  WHERE (([a].[Country]<>'ru') OR ([a].[Country]='ru') AND (NOT [a].[Issuer] LIKE '%восход%'))
    AND ([a].[NextUpdate]>=GETDATE())

  SELECT
     [a].[Country]
    ,[a].[Subject]
    ,COUNT([a].[Subject]) [Count]
  INTO #Crl
  FROM #CrlPairs [a]
  GROUP BY [a].[Country],[a].[Subject]

  SELECT
     [a].[Country]
    ,[d].[RussianShortName]
    ,CASE
      WHEN ([b].[Issuer] IS NULL) AND EXISTS(SELECT [o].[Issuer] FROM [dbo].[ReportCertificateRevocationList] [o] WHERE [o].[Country]=[a].[Country]) THEN '{ERROR}:Истек срок действия'
      WHEN [b].[Issuer] IS NULL       THEN '{ERROR}:Нет найдены'
      ELSE '{OK}'
      END [Status]
    ,ISNULL([b].[Issuer],(SELECT TOP 1 [o].[Issuer] FROM [dbo].[ReportCertificateRevocationList] [o] WHERE [o].[Country]=[a].[Country] ORDER BY [o].[NextUpdate] DESC)) [Issuer]
    ,ISNULL([b].[EffectiveDate],(SELECT TOP 1 [o].[EffectiveDate] FROM [dbo].[ReportCertificateRevocationList] [o] WHERE [o].[Country]=[a].[Country] ORDER BY [o].[NextUpdate] DESC)) [EffectiveDate]
    ,ISNULL([b].[NextUpdate],(SELECT TOP 1 [o].[NextUpdate] FROM [dbo].[ReportCertificateRevocationList] [o] WHERE [o].[Country]=[a].[Country] ORDER BY [o].[NextUpdate] DESC)) [NextUpdate]
    ,[c].[Count] [IssuerCount]
  FROM #Country [a]
    LEFT JOIN #Crl [c] ON [c].[Country]=[a].[Country]
    LEFT JOIN [dbo].[ReportCertificateRevocationList] [b] ON [b].[ObjectId]=[c].[Subject]
    INNER JOIN [dbo].[Country] [d] ON [d].[Country]=[a].[Country]
  ORDER BY [Country],[EffectiveDate],[NextUpdate]

  DROP TABLE #CrlPairs
  DROP TABLE #Crl
  DROP TABLE #Country
END