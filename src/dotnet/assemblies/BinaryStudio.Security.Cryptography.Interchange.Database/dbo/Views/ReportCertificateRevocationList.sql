




--<ScriptOptions statementTerminator="GO"/>

CREATE VIEW [dbo].[ReportCertificateRevocationList]
AS
SELECT
   [a].[ObjectId]
  ,[a].[Country]
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
  ,(SELECT
      [d].[Value]
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[AuthorityKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
      INNER JOIN [dbo].[GeneralName] [d] ON ([d].[GeneralNameId]=[c].[Issuer])
    WHERE [b].[ObjectId] = [a].[ObjectId]) [AuthorityKeyIssuer]
  ,(SELECT
      [c].[SerialNumber]
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[AuthorityKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [AuthorityKeySerialNumber]
FROM [dbo].[CertificateRevocationList] [a]