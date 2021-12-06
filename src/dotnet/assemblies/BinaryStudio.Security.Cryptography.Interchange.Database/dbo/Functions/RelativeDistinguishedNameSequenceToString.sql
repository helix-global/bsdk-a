CREATE FUNCTION [dbo].[RelativeDistinguishedNameSequenceToString] 
  (
	@RelativeDistinguishedNameSequenceId [int],
  @PartType NVARCHAR(MAX) = NULL
  )
RETURNS NVARCHAR(MAX)
AS
BEGIN
  DECLARE @Result NVARCHAR(MAX)=''
  IF @PartType IS NULL
  BEGIN
    DECLARE @Index  INT = 0
    DECLARE @Type   NVARCHAR(MAX)
    DECLARE @Value  NVARCHAR(MAX)
	  DECLARE [cursor] CURSOR LOCAL FOR
    SELECT
      COALESCE([c].[ShortName],[c].[Value]),
      [d].[Value]
    FROM [RelativeDistinguishedNameSequenceMapping] [a]
      INNER JOIN [RelativeDistinguishedName] [b] ON ([a].[RelativeDistinguishedNameId]=[b].[RelativeDistinguishedNameId])
      INNER JOIN [ObjectIdentifier]          [c] ON ([b].[Type]=[c].[Id])
      INNER JOIN [String]                    [d] ON ([b].[Value]=[d].[Id])
    WHERE ([a].[RelativeDistinguishedNameSequenceId]=@RelativeDistinguishedNameSequenceId)
    OPEN [cursor]
    FETCH NEXT FROM [cursor] INTO @Type,@Value
    WHILE (@@FETCH_STATUS = 0)
      BEGIN
        IF @Index <> 0 SET @Result = @Result + N','
        SET @Result = @Result + @Type + N'=' + @Value
        SET @Index = @Index + 1
        FETCH NEXT FROM [cursor] INTO @Type,@Value
      END
    CLOSE [cursor]
    DEALLOCATE [cursor]
  END
  ELSE BEGIN
    SELECT
      @Result = [d].[Value]
    FROM [RelativeDistinguishedNameSequenceMapping] [a]
      INNER JOIN [RelativeDistinguishedName] [b] ON ([a].[RelativeDistinguishedNameId]=[b].[RelativeDistinguishedNameId])
      INNER JOIN [ObjectIdentifier]          [c] ON ([b].[Type]=[c].[Id])
      INNER JOIN [String]                    [d] ON ([b].[Value]=[d].[Id])
    WHERE ([a].[RelativeDistinguishedNameSequenceId]=@RelativeDistinguishedNameSequenceId)
      AND (([c].[Value]=@PartType) OR ([c].[ShortName]=@PartType))
  END
  RETURN @Result
END
