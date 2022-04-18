-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE ImportRelativeDistinguishedName
  @Type NVARCHAR(MAX),
  @Value NVARCHAR(MAX),
  @Identifier INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  SET @Identifier = NULL
  BEGIN TRANSACTION
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
  COMMIT
END