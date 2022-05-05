-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetShortKey] 
  (
  @Value NVARCHAR(MAX)
  )
RETURNS NVARCHAR(4)
AS
BEGIN
  DECLARE @Length INT=LEN(@Value)
  IF @Length > 4
  BEGIN
    RETURN LEFT(@Value,2)+RIGHT(@Value,2)
  END
  RETURN @Value
END