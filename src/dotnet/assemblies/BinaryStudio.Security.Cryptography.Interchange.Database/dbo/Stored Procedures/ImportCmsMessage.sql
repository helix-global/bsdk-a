

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportCmsMessage]
  @Key AS NVARCHAR(MAX),
  @Body AS XML
AS
BEGIN
  SET NOCOUNT ON;
  --IF (@Key IS NOT NULL) AND (@Body IS NOT NULL)
  BEGIN
    --INSERT INTO Table_1 ([VALUE],[KEY]) VALUES (@Body,@Key);
    BEGIN TRANSACTION
      DECLARE @HashAlgorithm NVARCHAR(MAX)
      DECLARE @ContentType NVARCHAR(MAX)
      DECLARE @HashAlgorithmId INT
      DECLARE @ContentTypeId INT

      SELECT TOP 1
         @HashAlgorithm  = [a].value(N'CmsMessage.ContentInfo[1]/CmsSignedDataContentInfo[1]/DigestAlgorithms[1]/X509AlgorithmIdentifier[1]/@Identifier' ,N'NVARCHAR(MAX)')
        ,@ContentType    = [a].value(N'@ContentType' ,N'NVARCHAR(MAX)')
      FROM @Body.nodes(N'CmsMessage') [a]([a])
      execute [dbo].[ImportObjectIdentifier] @Value=@HashAlgorithm,@Identifier=@HashAlgorithmId output
      execute [dbo].[ImportObjectIdentifier] @Value=@ContentType,@Identifier=@ContentTypeId output

      DECLARE @ObjectId  INT
      INSERT INTO [dbo].[Object] ([Type],[Body],[Status],[Key]) VALUES (3,@Body,1,@Key)
      SET @ObjectId=@@IDENTITY
      INSERT INTO [dbo].[CmsMessage]
        ([ObjectId],[ContentType],[HashAlgorithm]) VALUES
        (@ObjectId, @ContentTypeId, @HashAlgorithmId)
      DECLARE @Certificate XML
      DECLARE [cursor] CURSOR LOCAL FOR
      SELECT
        [b].query(N'.') [Extension]
      FROM @Body.nodes(N'//Certificates') [a]([a])
        CROSS APPLY [a].nodes('Certificate') [b]([b])
      OPEN [cursor]
      FETCH NEXT FROM [cursor] INTO @Certificate
      WHILE (@@FETCH_STATUS = 0)
        BEGIN
          DECLARE @CertificateId INT
          EXECUTE [dbo].[ImportCertificate]
             @Thumbprint = NULL
            ,@Body=@Certificate
            ,@Status=1
            ,@CertificateId=@CertificateId OUTPUT
          INSERT INTO [dbo].[CmsCertificate] ([MessageId],[CertificateId]) VALUES (@ObjectId, @CertificateId)
          FETCH NEXT FROM [cursor] INTO @Certificate
        END
      CLOSE [cursor]
      DEALLOCATE [cursor]
      DECLARE @SignerInfo XML
      DECLARE [cursor] CURSOR LOCAL FOR
      SELECT
        [a].query(N'.')
      FROM @Body.nodes(N'//CmsSignerInfo') [a]([a])
      OPEN [cursor]
      FETCH NEXT FROM [cursor] INTO @SignerInfo
      WHILE (@@FETCH_STATUS = 0)
        BEGIN
          DECLARE @SignerInfoId INT
          EXECUTE [dbo].[ImportCmsSignerInfo]
             @MessageId = @ObjectId
            ,@Body=@SignerInfo
          FETCH NEXT FROM [cursor] INTO @SignerInfo
        END
      CLOSE [cursor]
      DEALLOCATE [cursor]
    COMMIT
  END
END