

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSubjectKeyIdentifier]
  @ExtensionId INT,
  @TraceLevel INT = 0
AS
BEGIN
  SET NOCOUNT ON;
  IF @ExtensionId IS NOT NULL
  BEGIN
    PRINT [dbo].[TraceIndentString](@TraceLevel,2) + N'Deleting SubjectKeyIdentifier {' + CAST(@ExtensionId AS NVARCHAR(MAX)) + '}'
  END
END