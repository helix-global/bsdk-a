﻿







CREATE VIEW [dbo].[ReportCertificate]
AS
SELECT
   [a].[ObjectId]
  ,[a].[Country]
  ,[a].[SerialNumber]
  ,(SELECT TOP 1 [b].[Value] FROM [dbo].[GeneralName] [b] WHERE [b].[GeneralNameId]=[a].[Issuer]) [Issuer]
  ,(SELECT TOP 1 [b].[Value] FROM [dbo].[GeneralName] [b] WHERE [b].[GeneralNameId]=[a].[Subject]) [Subject]
  ,(SELECT
      [c].[ShortKey]
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[SubjectKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [SubjectKeyIdentifier]
  ,(SELECT
      [c].[ShortKey]
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[AuthorityKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [AuthorityKeyIdentifier]
  ,[a].[Thumbprint] [Thumbprint]
  ,(SELECT TOP 1 [dbo].[OidToStr]([b].[Value]) FROM [dbo].[ObjectIdentifier] [b] WHERE [b].[Id]=[a].[SignatureAlgorithm]) [SignatureAlgorithm]
  ,(SELECT TOP 1 [dbo].[OidToStr]([b].[Value]) FROM [dbo].[ObjectIdentifier] [b] WHERE [b].[Id]=[a].[HashAlgorithm]) [HashAlgorithm]
  ,[a].[NotBefore]
  ,[a].[NotAfter]
FROM [dbo].[Certificate] [a]
  INNER JOIN [dbo].[Object] [o] ON [o].[ObjectId]=[a].[ObjectId]
--WHERE ([o].[Group] IS NULL)