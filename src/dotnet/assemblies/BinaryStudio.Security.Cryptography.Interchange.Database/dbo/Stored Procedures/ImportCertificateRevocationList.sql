
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportCertificateRevocationList]
  @Thumbprint AS NVARCHAR(MAX),
  @Body AS XML,
  @Group NVARCHAR(50)=NULL
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    IF @@TRANCOUNT = 0 RAISERROR('No external transaction', 16, 1)
    DECLARE @ObjectId INT
    DECLARE @IssuerId INT
    DECLARE @EffectiveDate DATETIME
    DECLARE @NextUpdate    DATETIME
    DECLARE @Country       VARCHAR(5)
    DECLARE @Extensions XML
    DECLARE @Issuer  XML
    IF (@Thumbprint IS NOT NULL) AND (@Body IS NOT NULL)
    BEGIN
      IF (NOT EXISTS(SELECT * FROM [dbo].[CertificateRevocationList] WHERE [Thumbprint] LIKE @Thumbprint))
      BEGIN
        SELECT TOP 1
           @EffectiveDate = [a].value(N'@EffectiveDate[1]',N'DATETIME')
          ,@NextUpdate    = [a].value(N'@NextUpdate[1]'   ,N'DATETIME')
          ,@Country       = [a].value(N'@Country[1]'      ,N'VARCHAR(5)')
          ,@Extensions = [a].query('Extensions')
          ,@Issuer  = [a].query('CertificateRevocationList.Issuer/RelativeDistinguishedName')
        FROM @Body.nodes(N'CertificateRevocationList') [a]([a])
        INSERT INTO [dbo].[Object] ([Type],[Body],[Group]) VALUES (2,@Body,@Group)
        SET @ObjectId = @@IDENTITY
        EXECUTE [dbo].[ImportRelativeDistinguishedNameSequence] @Body=@Issuer ,@Identifier=@IssuerId  OUTPUT
        INSERT INTO [dbo].[CertificateRevocationList]
                  ([ObjectId],[Country],[EffectiveDate],[NextUpdate],[Thumbprint],[Issuer])
          VALUES (@ObjectId, @Country, @EffectiveDate, @NextUpdate, @Thumbprint,@IssuerId)
        EXECUTE [dbo].[ImportExtensions] @ObjectId,@Body=@Extensions
      END
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