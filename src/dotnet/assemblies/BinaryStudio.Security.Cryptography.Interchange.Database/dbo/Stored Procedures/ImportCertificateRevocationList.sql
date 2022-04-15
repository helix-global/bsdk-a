
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
	--SET NOCOUNT ON;
  DECLARE @ObjectId AS INT
  DECLARE @EffectiveDate DATETIME
  DECLARE @NextUpdate    DATETIME
  DECLARE @Country       VARCHAR(5)
  IF (@Thumbprint IS NOT NULL) AND (@Body IS NOT NULL)
  BEGIN
    IF (NOT EXISTS(SELECT * FROM [dbo].[CertificateRevocationList] WHERE [Thumbprint] LIKE @Thumbprint))
    BEGIN
      SELECT TOP 1
         @EffectiveDate = [a].value(N'@EffectiveDate[1]',N'DATETIME')
        ,@NextUpdate    = [a].value(N'@NextUpdate[1]'   ,N'DATETIME')
        ,@Country       = [a].value(N'@Country[1]'      ,N'VARCHAR(5)')
      FROM @Body.nodes(N'CertificateRevocationList') [a]([a])
      INSERT INTO [dbo].[Object] ([Type],[Body]) VALUES (2,@Body)
      SET @ObjectId = @@IDENTITY
      INSERT INTO [dbo].[CertificateRevocationList]
               ([ObjectId],[Country],[EffectiveDate],[NextUpdate],[Thumbprint])
        VALUES (@ObjectId, @Country, @EffectiveDate, @NextUpdate, @Thumbprint)
    END
  END
END