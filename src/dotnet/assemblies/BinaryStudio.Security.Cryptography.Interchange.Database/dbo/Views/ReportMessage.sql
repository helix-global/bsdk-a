






CREATE VIEW [dbo].[ReportMessage]
AS
SELECT
   (SELECT TOP 1 [dbo].[OidToStr]([o].[Value]) FROM [dbo].[ObjectIdentifier] [o] WHERE [o].[Id]=[a].[ContentType]) [ContentType]
  ,(SELECT TOP 1 [o].[CertificateId] FROM [dbo].[CmsCertificate] [o] WHERE [o].[MessageId]=[a].[ObjectId]) [Certificate]
  ,(SELECT TOP 1 [dbo].[OidToStr]([o].[Value]) FROM [dbo].[ObjectIdentifier] [o] WHERE [o].[Id]=[a].[HashAlgorithm]) [ContentHashAlgorithm]
  ,[b].[Key] [Key]
  ,(SELECT TOP 1 [dbo].[OidToStr]([o].[Value]) FROM [dbo].[ObjectIdentifier] [o] WHERE [o].[Id]=[c].[HashAlgorithm]) [SignerHashAlgorithm]
  ,(SELECT TOP 1 [dbo].[OidToStr]([o].[Value]) FROM [dbo].[ObjectIdentifier] [o] WHERE [o].[Id]=[c].[SignatureAlgorithm]) [SignerSignatureAlgorithm]
  ,(SELECT TOP 1 [o].[Value] FROM [dbo].[GeneralName] [o] WHERE [o].[GeneralNameId]=[c].[Issuer]) [SignerIssuer]
  ,[c].[IssuerSerialNumber]
  ,[c].[SigningTime]
  ,[a].[Thumbprint]
FROM [dbo].[CmsMessage] [a] WITH(NOLOCK)
  INNER JOIN [dbo].[Object] [b] ON [b].[ObjectId]=[a].[ObjectId]
  INNER JOIN [dbo].[CmsSignerInfo] [c] ON [c].[MessageId]=[a].[ObjectId]