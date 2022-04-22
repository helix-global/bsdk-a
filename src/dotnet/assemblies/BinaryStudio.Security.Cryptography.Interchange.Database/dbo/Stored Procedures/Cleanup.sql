-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Cleanup]
AS
BEGIN
  --DELETE FROM [dbo].[AlternativeName]
  --DELETE FROM [dbo].[AuthorityKeyIdentifier]
  --DELETE FROM [dbo].[IssuerAlternativeName]
  --DELETE FROM [dbo].[SubjectAlternativeName]
  --DELETE FROM [dbo].[SubjectKeyIdentifier]
  --DELETE FROM [dbo].[Extension]
  --DELETE FROM [dbo].[RelativeDistinguishedNameSequenceMapping]
  --DELETE FROM [dbo].[Certificate]
  --DELETE FROM [dbo].[CertificateRevocationList]
  --DELETE FROM [dbo].[RelativeDistinguishedName]
  --DELETE FROM [dbo].[RelativeDistinguishedNameSequence]
  --DELETE FROM [dbo].[GeneralName]
  --DELETE FROM [dbo].[Object]
  --DELETE FROM [dbo].[ObjectIdentifier]
  --DELETE FROM [dbo].[String]
  DELETE FROM [dbo].[CmsCertificate]
  DELETE FROM [dbo].[CmsSignerInfo]
  DELETE FROM [dbo].[CmsMessage]
  DELETE FROM [dbo].[Object] WHERE [ObjectId]=3
  SET NOCOUNT ON;
END