
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteAuthorityKeyIdentifier]
  @ExtensionId INT,
  @TraceLevel INT = 0
AS
BEGIN
  SET NOCOUNT ON;
  IF @ExtensionId IS NOT NULL
  BEGIN
    PRINT [dbo].[TraceIndentString](@TraceLevel,2) + N'Deleting AuthorityKeyIdentifier {' + CAST(@ExtensionId AS NVARCHAR(MAX)) + '}'
    DELETE FROM [dbo].[AuthorityKeyIdentifier] WHERE [ExtensionId]=@ExtensionId
  END
END