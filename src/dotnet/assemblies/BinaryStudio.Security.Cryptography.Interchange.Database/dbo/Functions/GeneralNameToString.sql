-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GeneralNameToString]
(
	@GeneralNameId INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
  DECLARE @Type INT
  DECLARE @Value NVARCHAR(MAX)
  SELECT TOP 1
    @Type  = [a].[Type],
    @Value = [a].[Value]
  FROM [dbo].[GeneralName] [a]
  WHERE [a].[GeneralNameId] = @GeneralNameId
  IF (@Value IS NULL) AND (@Type = 4)
  BEGIN
    SET @Value = [dbo].[RelativeDistinguishedNameSequenceToString](@GeneralNameId,NULL)
  END
  RETURN @Value
END
