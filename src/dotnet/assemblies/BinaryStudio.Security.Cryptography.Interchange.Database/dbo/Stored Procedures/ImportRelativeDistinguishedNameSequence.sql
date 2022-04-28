-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportRelativeDistinguishedNameSequence]
  @Body XML,
  @Identifier INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  SET @Identifier = NULL
  BEGIN TRY
    IF @@TRANCOUNT = 0 RAISERROR('No external transaction', 16, 1)
    if @Body is not null
    begin
      DECLARE @Value NVARCHAR(MAX)
      DECLARE @RelativeDistinguishedNameSequenceId INT
      SELECT TOP 1
          @Value = [a].value(N'@Value[1]',N'NVARCHAR(MAX)')
      FROM @Body.nodes(N'RelativeDistinguishedName') [a]([a])
      SELECT
        @RelativeDistinguishedNameSequenceId=[a].[RelativeDistinguishedNameSequenceId]
      FROM [dbo].[RelativeDistinguishedNameSequence] [a]
        INNER JOIN [dbo].[GeneralName] [b] ON [b].[GeneralNameId]=[a].[RelativeDistinguishedNameSequenceId]
      WHERE [b].[Value] LIKE @Value
      IF @RelativeDistinguishedNameSequenceId IS NOT NULL
      BEGIN
        SET @Identifier=@RelativeDistinguishedNameSequenceId
        RETURN
      END
      INSERT INTO [dbo].[GeneralName] ([Type],[Value]) VALUES (4,@Value)
      SET @RelativeDistinguishedNameSequenceId=@@IDENTITY
      INSERT INTO [dbo].[RelativeDistinguishedNameSequence] ([RelativeDistinguishedNameSequenceId]) VALUES (@RelativeDistinguishedNameSequenceId)
      DECLARE @Type NVARCHAR(MAX)
      DECLARE @RelativeDistinguishedNameId INT
      DECLARE [cursor] CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
      SELECT
        [b].value(N'@Type[1]',N'NVARCHAR(MAX)') [Type],
        [b].value(N'@Value[1]',N'NVARCHAR(MAX)') [Value]
      FROM @Body.nodes(N'RelativeDistinguishedName') [a]([a])
      CROSS APPLY [a].nodes(N'Attribute') [b]([b])
      OPEN [cursor]
      FETCH NEXT FROM [cursor] INTO @Type,@Value
      WHILE (@@FETCH_STATUS = 0)
        BEGIN
          EXECUTE [dbo].[ImportRelativeDistinguishedName]
              @Type
            ,@Value
            ,@Identifier=@RelativeDistinguishedNameId OUTPUT
          INSERT INTO [RelativeDistinguishedNameSequenceMapping] ([RelativeDistinguishedNameSequenceId],[RelativeDistinguishedNameId]) VALUES
            (@RelativeDistinguishedNameSequenceId,@RelativeDistinguishedNameId)
          FETCH NEXT FROM [cursor] INTO @Type,@Value
        END
      CLOSE [cursor]
      DEALLOCATE [cursor]
      SET @Identifier=@RelativeDistinguishedNameSequenceId
    end
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