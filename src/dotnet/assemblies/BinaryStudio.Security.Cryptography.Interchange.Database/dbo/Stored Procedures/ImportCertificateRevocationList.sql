
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportCertificateRevocationList]
  @Thumbprint AS NVARCHAR(MAX),
	@Body AS XML
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @ObjectId INT
  DECLARE @IssuerId INT
  DECLARE @EffectiveDate DATETIME
  DECLARE @NextUpdate    DATETIME
  DECLARE @Country       VARCHAR(5)
  DECLARE @Extensions XML
  DECLARE @Issuer  XML
  IF (@Thumbprint IS NOT NULL) AND (@Body IS NOT NULL)
  BEGIN
    --INSERT INTO Table_1 ([VALUE]) VALUES (@Body);
    IF (NOT EXISTS(SELECT * FROM [dbo].[CertificateRevocationList] WHERE [Thumbprint] LIKE @Thumbprint))
    BEGIN
      BEGIN TRANSACTION
        SELECT TOP 1
           @EffectiveDate = [a].value(N'@EffectiveDate[1]',N'DATETIME')
          ,@NextUpdate    = [a].value(N'@NextUpdate[1]'   ,N'DATETIME')
          ,@Country       = [a].value(N'@Country[1]'      ,N'VARCHAR(5)')
          ,@Extensions = [a].query('Extensions')
          ,@Issuer  = [a].query('CertificateRevocationList.Issuer/RelativeDistinguishedName')
        FROM @Body.nodes(N'CertificateRevocationList') [a]([a])
        INSERT INTO [dbo].[Object] ([Type],[Body]) VALUES (2,@Body)
        SET @ObjectId = @@IDENTITY
        EXECUTE [dbo].[ImportRelativeDistinguishedNameSequence] @Body=@Issuer ,@Identifier=@IssuerId  OUTPUT
        INSERT INTO [dbo].[CertificateRevocationList]
                 ([ObjectId],[Country],[EffectiveDate],[NextUpdate],[Thumbprint],[Issuer])
          VALUES (@ObjectId, @Country, @EffectiveDate, @NextUpdate, @Thumbprint,@IssuerId)
        EXECUTE [dbo].[ImportExtensions] @ObjectId,@Body=@Extensions
      COMMIT
    END
  END
END