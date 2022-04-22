
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportCertificate]
  @Thumbprint AS NVARCHAR(MAX),
  @Body AS XML,
  @Status TINYINT=NULL,
  @CertificateId INT=NULL OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  IF (@Thumbprint IS NULL) AND (@Body IS NOT NULL)
  BEGIN
    SELECT TOP 1
      @Thumbprint  = [a].value(N'@Thumbprint[1]' ,N'NVARCHAR(MAX)')
    FROM @Body.nodes(N'Certificate') [a]([a])
  END
  IF (@Thumbprint IS NOT NULL) AND (@Body IS NOT NULL)
  BEGIN
    --INSERT INTO Table_1 ([VALUE]) VALUES (@Body);
    SELECT
      @CertificateId=[ObjectId]
    FROM [dbo].[Certificate] WHERE [Thumbprint] LIKE @Thumbprint
    IF (@CertificateId IS NULL)
    BEGIN
      BEGIN TRANSACTION
        DECLARE @ObjectId  INT
        DECLARE @IssuerId  INT
        DECLARE @SubjectId INT
        DECLARE @SignatureAlgorithmId INT
        DECLARE @HashAlgorithmId INT
        DECLARE @NotBefore DATETIME
        DECLARE @NotAfter  DATETIME
        DECLARE @Issuer  XML
        DECLARE @Subject XML
        DECLARE @Country VARCHAR(5)
        DECLARE @SerialNumber VARCHAR(MAX)
        DECLARE @SignatureAlgorithm VARCHAR(MAX)
        DECLARE @HashAlgorithm VARCHAR(MAX)
        DECLARE @Extensions XML
        SELECT TOP 1
           @Issuer  = [a].query('Certificate.Issuer/RelativeDistinguishedName')
          ,@Subject = [a].query('Certificate.Subject/RelativeDistinguishedName')
          ,@Extensions = [a].query('Extensions')
          ,@NotAfter  = [a].value(N'@NotAfter[1]' ,N'DATETIME')
          ,@NotBefore = [a].value(N'@NotBefore[1]',N'DATETIME')
          ,@Country   = [a].value(N'@Country[1]'  ,N'VARCHAR(5)')
          ,@SerialNumber = [a].value(N'@SerialNumber[1]'  ,N'VARCHAR(MAX)')
          ,@SignatureAlgorithm = [a].value(N'Certificate.SignatureAlgorithm[1]/*[1]/@SignatureAlgorithm[1]'  ,N'VARCHAR(MAX)')
          ,@HashAlgorithm = [a].value(N'Certificate.SignatureAlgorithm[1]/*[1]/@HashAlgorithm[1]'  ,N'VARCHAR(MAX)')
        FROM @Body.nodes(N'Certificate') [a]([a])
        EXECUTE [dbo].[ImportRelativeDistinguishedNameSequence] @Body=@Issuer ,@Identifier=@IssuerId  OUTPUT
        EXECUTE [dbo].[ImportRelativeDistinguishedNameSequence] @Body=@Subject,@Identifier=@SubjectId OUTPUT
        execute [dbo].[ImportObjectIdentifier] @Value=@SignatureAlgorithm,@Identifier=@SignatureAlgorithmId output
        execute [dbo].[ImportObjectIdentifier] @Value=@HashAlgorithm,@Identifier=@HashAlgorithmId output
        INSERT INTO [dbo].[Object] ([Type],[Body],[Status]) VALUES (1,@Body,@Status)
        SET @ObjectId=@@IDENTITY
        INSERT INTO [dbo].[Certificate]
          ([ObjectId],[Country],[SerialNumber],[Thumbprint],[Issuer], [Subject], [NotBefore],[NotAfter],[SignatureAlgorithm],[HashAlgorithm]) VALUES
          (@ObjectId, @Country, @SerialNumber, @Thumbprint, @IssuerId,@SubjectId,@NotBefore, @NotAfter,@SignatureAlgorithmId,@HashAlgorithmId)
        EXECUTE [dbo].[ImportExtensions] @ObjectId,@Body=@Extensions
      COMMIT
      SET @CertificateId=@ObjectId
    END
  END
END