-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportObjectIdentifier]
  @Value NVARCHAR(MAX),
  @Identifier INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  SET @Identifier = NULL
  IF @Value IS NOT NULL
  BEGIN TRANSACTION
    SELECT TOP 1 @Identifier=[a].[Id] FROM [dbo].[ObjectIdentifier] [a] WHERE ([a].[Value]=@Value)
    IF @Identifier IS NULL
    BEGIN
      INSERT INTO [dbo].[ObjectIdentifier] ([Value]) VALUES (@Value)
      SET @Identifier=@@IDENTITY
    END
  COMMIT
END