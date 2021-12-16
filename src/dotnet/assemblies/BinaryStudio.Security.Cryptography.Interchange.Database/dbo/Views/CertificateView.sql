CREATE VIEW "dbo"."CertificateView"
AS
SELECT
   [a].[ObjectId]
  ,[a].[Country]
  ,[a].[SerialNumber]
  ,[dbo].[RelativeDistinguishedNameSequenceToString]([a].[Issuer],NULL)  [Issuer]
  ,[dbo].[RelativeDistinguishedNameSequenceToString]([a].[Subject],NULL) [Subject]
  ,(SELECT
      [dbo].[string_agg]([dbo].[GeneralNameToString]([e].[GeneralNameId]),';','{','}')
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[SubjectAlternativeName] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
      LEFT  JOIN [dbo].[AlternativeName]        [d] ON [d].[ExtensionId]=[b].[ExtensionId]
      INNER JOIN [dbo].[GeneralName]            [e] ON [e].[GeneralNameId]=[d].[GeneralNameId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [SubjectAlternativeName]
  ,(SELECT
      [dbo].[string_agg]([dbo].[GeneralNameToString]([e].[GeneralNameId]),';','{','}')
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[IssuerAlternativeName] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
      LEFT  JOIN [dbo].[AlternativeName]        [d] ON [d].[ExtensionId]=[b].[ExtensionId]
      INNER JOIN [dbo].[GeneralName]            [e] ON [e].[GeneralNameId]=[d].[GeneralNameId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [IssuerAlternativeName]
  ,(SELECT
      [c].[ShortKey]
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[SubjectKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [SubjectKeyIdentifier]
  ,(SELECT
      [c].[ShortKey]
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[AuthorityKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [AuthorityKeyIdentifier.Key]
  ,(SELECT
      [c].[SerialNumber]
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[AuthorityKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [AuthorityKeyIdentifier.SerialNumber]
  ,(SELECT
      [dbo].[GeneralNameToString]([c].[Issuer])
    FROM [dbo].[Extension] [b]
      INNER JOIN [dbo].[AuthorityKeyIdentifier] [c] ON [c].[ExtensionId]=[b].[ExtensionId]
    WHERE [b].[ObjectId] = [a].[ObjectId]) [AuthorityKeyIdentifier.Issuer]
FROM [dbo].[Certificate] [a]
GO



GO


