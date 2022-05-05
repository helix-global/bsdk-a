


--<ScriptOptions statementTerminator="GO"/>

CREATE VIEW [dbo].[ReportCertificateRevocationList]
AS
SELECT
   [a].[Country]
  ,[a].[EffectiveDate]
  ,[a].[NextUpdate]
  ,(SELECT TOP 1 [b].[Value]
    FROM [dbo].[GeneralName] [b] WHERE [b].[GeneralNameId]=[a].[Issuer]) [Issuer]
  ,(SELECT
      [c].[ShortKey]
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[AuthorityKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [AuthorityKeyIdentifier]
  ,[a].[Thumbprint] [Thumbprint]
FROM [dbo].[CertificateRevocationList] [a]