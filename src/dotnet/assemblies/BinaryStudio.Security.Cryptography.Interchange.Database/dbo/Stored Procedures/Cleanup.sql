-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Cleanup]
  @DeleteCrl BIT=0
AS
BEGIN
  SET NOCOUNT ON;
  IF EXISTS(SELECT * FROM [dbo].[CertificateRevocationList])
  BEGIN
    IF @DeleteCrl = 1
    BEGIN
      PRINT N'Deleting Crl...'
      DECLARE @ObjectId INT
      DECLARE [c] CURSOR LOCAL FOR
        SELECT
          [a].[ObjectId]
        FROM [dbo].[CertificateRevocationList] [a]
      OPEN [c]
      FETCH NEXT FROM [c] INTO @ObjectId
      WHILE @@FETCH_STATUS = 0
      BEGIN
        EXECUTE [dbo].[DeleteCertificateRevocationList] @ObjectId=@ObjectId, @TraceLevel=1
        FETCH NEXT FROM [c] INTO @ObjectId
      END
      CLOSE [c]
      DEALLOCATE [c]
    END
  END
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
  --DELETE FROM [dbo].[CmsCertificate]
  --DELETE FROM [dbo].[CmsSignerInfo]
 -- DELETE FROM [dbo].[CmsMessage]
 -- FROM [dbo].[CmsMessage] [a]
	--INNER JOIN [dbo].[Object] [b] ON ([b].[ObjectId]=[a].[ObjectId]) AND ([b].[Type]=3)
 -- WHERE [b].[Key] IS NULL
 -- --DELETE FROM [dbo].[Object] WHERE [ObjectId]=3
END