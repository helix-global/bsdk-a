﻿

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportCmsMessage]
  @Key AS NVARCHAR(MAX),
  @Body AS XML,
  @Thumbprint AS NVARCHAR(MAX),
  @Group AS TINYINT
AS
BEGIN
  SET NOCOUNT ON;
  IF (@Key IS NOT NULL) AND (@Body IS NOT NULL)
  BEGIN
    IF (NOT EXISTS(SELECT TOP 1
        [a].[ObjectId]
       FROM [dbo].[CmsMessage] [a]
        INNER JOIN [dbo].[Object] [b] ON [b].[ObjectId]=[a].[ObjectId]
      WHERE ([b].[Key]=@Key) AND ([b].[Group]=@Group)))
    BEGIN
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
        INSERT INTO [dbo].[Object] ([Type],[Body],[Group],[Key]) VALUES (3,@Body,@Group,@Key)
        SET @ObjectId=@@IDENTITY
        INSERT INTO [dbo].[CmsMessage]
          ([ObjectId],[ContentType],[HashAlgorithm],[Thumbprint]) VALUES
          (@ObjectId, @ContentTypeId, @HashAlgorithmId,@Thumbprint)
        DECLARE @Certificate XML
        DECLARE [cursor] CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
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
              ,@Group=@Group
              ,@CertificateId=@CertificateId OUTPUT
            INSERT INTO [dbo].[CmsCertificate] ([MessageId],[CertificateId]) VALUES (@ObjectId, @CertificateId)
            FETCH NEXT FROM [cursor] INTO @Certificate
          END
        CLOSE [cursor]
        DEALLOCATE [cursor]
        DECLARE @SignerInfo XML
        DECLARE [cursor] CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
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
	ELSE IF (@Thumbprint IS NOT NULL) AND (@Key IS NOT NULL)
	BEGIN
	  UPDATE [a]
      SET [a].[Thumbprint]=@Thumbprint
    FROM [dbo].[CmsMessage] [a]
      INNER JOIN [dbo].[Object] [b] ON [b].[ObjectId]=[a].[ObjectId]
    WHERE ([b].[Key]=@Key)
      AND ([b].[Group]=@Group)
      AND ([a].[Thumbprint] IS NULL)
	  RETURN
	END
  END
END