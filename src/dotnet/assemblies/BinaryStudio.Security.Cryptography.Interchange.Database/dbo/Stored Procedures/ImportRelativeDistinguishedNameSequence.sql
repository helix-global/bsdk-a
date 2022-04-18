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
    BEGIN TRANSACTION
      INSERT INTO [dbo].[GeneralName] ([Type],[Value]) VALUES (4,@Value)
      SET @RelativeDistinguishedNameSequenceId=@@IDENTITY
      INSERT INTO [dbo].[RelativeDistinguishedNameSequence] ([RelativeDistinguishedNameSequenceId]) VALUES (@RelativeDistinguishedNameSequenceId)
      DECLARE @Type NVARCHAR(MAX)
      DECLARE @RelativeDistinguishedNameId INT
      DECLARE [cursor] CURSOR LOCAL FOR
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
    COMMIT
  end
END