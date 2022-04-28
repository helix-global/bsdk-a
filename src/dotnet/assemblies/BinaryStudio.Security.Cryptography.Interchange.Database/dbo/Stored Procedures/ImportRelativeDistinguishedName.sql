-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportRelativeDistinguishedName]
  @Type NVARCHAR(MAX),
  @Value NVARCHAR(MAX),
  @Identifier INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  SET @Identifier = NULL
  BEGIN TRY
    IF @@TRANCOUNT = 0 RAISERROR('No external transaction', 16, 1)
    DECLARE @TypeId INT
    DECLARE @ValueId INT
    SELECT TOP 1 @TypeId=[a].[Id] FROM [dbo].[ObjectIdentifier] [a] WHERE ([a].[Value]=@Type)
    IF @TypeId IS NULL
    BEGIN
      INSERT INTO [dbo].[ObjectIdentifier] ([Value]) VALUES (@Type)
      SET @TypeId=@@IDENTITY
    END
    SELECT TOP 1 @ValueId=[a].[Id] FROM [dbo].[String] [a] WHERE ([a].[Value] LIKE @Value)
    IF @ValueId IS NULL
    BEGIN
      INSERT INTO [dbo].[String] ([Value]) VALUES (@Value)
      SET @ValueId=@@IDENTITY
    END
    SELECT
      @Identifier=[a].[RelativeDistinguishedNameId]
    FROM [dbo].[RelativeDistinguishedName] [a]
    WHERE ([a].[Type] = @TypeId) AND ([a].[Value] = @ValueId)
    IF @Identifier IS NULL
    BEGIN
      INSERT INTO [dbo].[RelativeDistinguishedName] ([Type],[Value]) VALUES (@TypeId,@ValueId)
      SET @Identifier=@@IDENTITY
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