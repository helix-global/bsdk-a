











CREATE VIEW [dbo].[ReportMessage]
AS
SELECT
   [cm].[ObjectId]
  ,(SELECT TOP 1 [cs].[CertificateId] FROM [dbo].[CmsCertificate] [cs] WHERE [cs].[MessageId]=[cm].[ObjectId]) [Certificate]
  ,(SELECT TOP 1 [dbo].[OidToStr]([oi].[Value]) FROM [dbo].[ObjectIdentifier] [oi] WHERE [oi].[Id]=[cm].[HashAlgorithm]) [ContentHashAlgorithm]
  ,[o].[Key] [Key]
  ,(SELECT TOP 1 [dbo].[OidToStr]([oi].[Value]) FROM [dbo].[ObjectIdentifier] [oi] WHERE [oi].[Id]=[c].[HashAlgorithm]) [SignerHashAlgorithm]
  ,(SELECT TOP 1 [dbo].[OidToStr]([oi].[Value]) FROM [dbo].[ObjectIdentifier] [oi] WHERE [oi].[Id]=[c].[SignatureAlgorithm]) [SignerSignatureAlgorithm]
  ,(SELECT TOP 1 [gn].[Value] FROM [dbo].[GeneralName] [gn] WHERE [gn].[GeneralNameId]=[c].[Issuer]) [SignerIssuer]
  ,[c].[IssuerSerialNumber]
  ,[c].[SigningTime]
  ,[cm].[Thumbprint]
  ,[o].[Group]
  ,[o].[ShortFileIdentifier]
FROM [dbo].[CmsMessage] [cm] WITH(NOLOCK)
  INNER 
	JOIN [dbo].[Object] [o] 
		ON [o].[ObjectId]=[cm].[ObjectId]
  INNER 
	JOIN [dbo].[CmsSignerInfo] [c] 
		ON [c].[MessageId]=[cm].[ObjectId]