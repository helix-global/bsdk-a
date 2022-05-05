﻿


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportCmsSignerInfo]
  @MessageId AS INT,
  @Body AS XML
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    IF @@TRANCOUNT = 0 RAISERROR('No external transaction', 16, 1)
    IF (@MessageId IS NOT NULL) AND (@Body IS NOT NULL)
    BEGIN
      DECLARE @IssuerId INT
      DECLARE @HashAlgorithmId INT
      DECLARE @SignatureAlgorithmId INT
      DECLARE @IssuerSerialNumber NVARCHAR(MAX)
      DECLARE @HashAlgorithm NVARCHAR(MAX)
      DECLARE @SignatureAlgorithm NVARCHAR(MAX)
      DECLARE @SigningTime DATETIME
      DECLARE @Issuer XML
      SELECT TOP 1
         @HashAlgorithm  = [a].value(N'CmsSignerInfo.DigestAlgorithm[1]/X509AlgorithmIdentifier[1]/@Identifier[1]' ,N'NVARCHAR(MAX)')
        ,@SignatureAlgorithm = [a].value(N'CmsSignerInfo.SignatureAlgorithm[1]/X509AlgorithmIdentifier[1]/@Identifier[1]' ,N'NVARCHAR(MAX)')
        ,@IssuerSerialNumber = [a].value(N'CmsSignerInfo.SignerIdentifier[1]/CmsIssuerAndSerialNumber[1]/@CertificateSerialNumber[1]'  ,N'VARCHAR(MAX)')
        ,@SigningTime = [a].value(N'SignedAttributes[1]/Attribute[@Type="1.2.840.113549.1.9.5"][1]/Attribute.Value[1]/Object[1]'  ,N'DATETIME')
        ,@Issuer = [a].query('//CmsIssuerAndSerialNumber.CertificateIssuer/RelativeDistinguishedName')
      FROM @Body.nodes(N'CmsSignerInfo') [a]([a])
      EXECUTE [dbo].[ImportRelativeDistinguishedNameSequence] @Body=@Issuer,@Identifier=@IssuerId OUTPUT
      execute [dbo].[ImportObjectIdentifier] @Value=@SignatureAlgorithm,@Identifier=@SignatureAlgorithmId output
      execute [dbo].[ImportObjectIdentifier] @Value=@HashAlgorithm,@Identifier=@HashAlgorithmId output
      INSERT INTO [dbo].[CmsSignerInfo]
                  ([MessageId]
                  ,[Issuer]
                  ,[IssuerSerialNumber]
                  ,[SignatureAlgorithm]
                  ,[HashAlgorithm]
                  ,[SigningTime])
            VALUES
                  (@MessageId
                  ,@IssuerId
                  ,@IssuerSerialNumber
                  ,@SignatureAlgorithmId
                  ,@HashAlgorithmId
                  ,@SigningTime)
    END
  END   TRY
  BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(MAX)
    DECLARE @ErrorSeverity NVARCHAR(MAX)
    DECLARE @ErrorState NVARCHAR(MAX)
    DECLARE @ErrorNumber NVARCHAR(MAX)
    DECLARE @ErrorLine NVARCHAR(MAX)
    DECLARE @ErrorProcedure NVARCHAR(MAX)

    SELECT
      @ErrorMessage   = ERROR_MESSAGE()
     ,@ErrorSeverity  = CAST(ERROR_SEVERITY() AS NVARCHAR(MAX))
     ,@ErrorState     = CAST(ERROR_STATE() AS NVARCHAR(MAX))
     ,@ErrorNumber    = CAST(ERROR_NUMBER() AS NVARCHAR(MAX))
     ,@ErrorLine      = CAST(ERROR_LINE() AS NVARCHAR(MAX))
     ,@ErrorProcedure = CAST(ERROR_PROCEDURE() AS NVARCHAR(MAX))

    SET @ErrorMessage = @ErrorMessage +
      N':{Number='+@ErrorNumber+
      '},{State='+ @ErrorState +
      '},{Severity='+ @ErrorSeverity +
      '},{Line='+ @ErrorLine +
      '},{Procedure='+ @ErrorProcedure +'}'
    IF @@TRANCOUNT > 0 ROLLBACK TRAN
    RAISERROR(@ErrorMessage, 11, 1)
  END   CATCH
END