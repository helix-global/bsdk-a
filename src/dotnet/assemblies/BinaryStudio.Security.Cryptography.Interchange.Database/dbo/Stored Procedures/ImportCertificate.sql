
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportCertificate]
  @Thumbprint AS NVARCHAR(MAX),
  @Body AS XML
AS
BEGIN
  SET NOCOUNT ON;
  IF (@Thumbprint IS NOT NULL) AND (@Body IS NOT NULL)
  BEGIN
    --INSERT INTO Table_1 ([VALUE]) VALUES (@Body);
    IF (NOT EXISTS(SELECT * FROM [dbo].[Certificate] WHERE [Thumbprint] LIKE @Thumbprint))
    BEGIN
      BEGIN TRANSACTION
        DECLARE @ObjectId  INT
        DECLARE @IssuerId  INT
        DECLARE @SubjectId INT
        DECLARE @NotBefore DATETIME
        DECLARE @NotAfter  DATETIME
        DECLARE @Issuer  XML
        DECLARE @Subject XML
        DECLARE @Country VARCHAR(5)
        DECLARE @SerialNumber VARCHAR(MAX)
        DECLARE @Extensions XML
        SELECT TOP 1
           @Issuer  = [a].query('Certificate.Issuer/RelativeDistinguishedName')
          ,@Subject = [a].query('Certificate.Subject/RelativeDistinguishedName')
          ,@Extensions = [a].query('Extensions')
          ,@NotAfter  = [a].value(N'@NotAfter[1]' ,N'DATETIME')
          ,@NotBefore = [a].value(N'@NotBefore[1]',N'DATETIME')
          ,@Country   = [a].value(N'@Country[1]'  ,N'VARCHAR(5)')
          ,@SerialNumber = [a].value(N'@SerialNumber[1]'  ,N'VARCHAR(MAX)')
        FROM @Body.nodes(N'Certificate') [a]([a])
        EXECUTE [dbo].[ImportRelativeDistinguishedNameSequence] @Body=@Issuer ,@Identifier=@IssuerId  OUTPUT
        EXECUTE [dbo].[ImportRelativeDistinguishedNameSequence] @Body=@Subject,@Identifier=@SubjectId OUTPUT
        INSERT INTO [dbo].[Object] ([Type],[Body]) VALUES (1,@Body)
        SET @ObjectId=@@IDENTITY
        INSERT INTO [dbo].[Certificate]
          ([ObjectId],[Country],[SerialNumber],[Thumbprint],[Issuer], [Subject], [NotBefore],[NotAfter]) VALUES
          (@ObjectId, @Country, @SerialNumber, @Thumbprint, @IssuerId,@SubjectId,@NotBefore, @NotAfter)
        EXECUTE [dbo].[ImportExtensions] @ObjectId,@Body=@Extensions
      COMMIT
    END
  END
END