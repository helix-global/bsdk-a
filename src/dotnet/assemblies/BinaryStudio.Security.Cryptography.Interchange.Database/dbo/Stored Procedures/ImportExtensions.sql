
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
  BEGIN TRY
    IF @@TRANCOUNT = 0 RAISERROR('No external transaction', 16, 1)
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