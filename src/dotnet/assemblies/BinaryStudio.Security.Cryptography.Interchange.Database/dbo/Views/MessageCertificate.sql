



CREATE VIEW [dbo].[MessageCertificate]
AS
SELECT
   [a].[Country]
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
FROM [dbo].[Certificate] [a]
  INNER JOIN [dbo].[Object] [o] ON [o].[ObjectId]=[a].[ObjectId]
WHERE ([o].[Status] = 1)