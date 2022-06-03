

CREATE VIEW [dbo].[ReportCertificate]
AS
WITH [T] AS
(
  SELECT
     [a].[ObjectId]
    ,[a].[Country]
    ,[a].[SerialNumber]
    ,(SELECT TOP 1 [b].[Value] FROM [dbo].[GeneralName] [b] WHERE [b].[GeneralNameId]=[a].[Issuer])  [Issuer]
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
  FROM [dbo].[Certificate] [a]
    INNER JOIN [dbo].[Object] [o] ON [o].[ObjectId]=[a].[ObjectId]
)
SELECT
   [a].[ObjectId]
  ,[a].[Country]
  ,[a].[SerialNumber]
  ,[a].[Issuer]
  ,[a].[Subject]
  ,[a].[SubjectKeyIdentifier]
  ,CASE ([dbo].[StringEquals]([a].[AuthorityKeyIdentifier],NULL,NULL))
    WHEN 0 THEN [a].[AuthorityKeyIdentifier]
    ELSE
      CASE ([dbo].[StringEquals]([a].[Issuer],[a].[Subject], 'OrdinalIgnoreCase'))
      WHEN 1 THEN [a].[SubjectKeyIdentifier]
      ELSE NULL
      END
    END [AuthorityKeyIdentifier1]
  ,[a].[AuthorityKeyIdentifier] [AuthorityKeyIdentifier]
  ,([dbo].[StringEquals]([a].[AuthorityKeyIdentifier],NULL,NULL)) [xxx]
  ,[a].[Thumbprint]
  ,[a].[SignatureAlgorithm]
  ,[a].[HashAlgorithm]
  ,[a].[NotBefore]
  ,[a].[NotAfter]
  ,[a].[AuthorityKeyIssuer]
  ,[a].[AuthorityKeySerialNumber] [IssuerSerialNumber]
FROM [T] [a]