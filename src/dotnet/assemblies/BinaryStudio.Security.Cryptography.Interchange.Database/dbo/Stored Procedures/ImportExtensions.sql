
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportExtensions]
  @ObjectId AS INT,
  @Body AS XML
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION
    DECLARE @Extension XML
    DECLARE [cursor] CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
    SELECT
      [a].query(N'.') [Extension]
    FROM @Body.nodes(N'//Extension') [a]([a])
    OPEN [cursor]
    FETCH NEXT FROM [cursor] INTO @Extension
    WHILE (@@FETCH_STATUS = 0)
      BEGIN
        EXECUTE [dbo].[ImportExtension] @ObjectId,@Body=@Extension
        FETCH NEXT FROM [cursor] INTO @Extension
      END
    CLOSE [cursor]
    DEALLOCATE [cursor]
  COMMIT
END