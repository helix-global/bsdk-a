-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteExtension]
  @ExtensionId INT,
  @TraceLevel INT = 0
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @InnerTraceLevel INT = @TraceLevel+1
  IF @ExtensionId IS NOT NULL
  BEGIN
    PRINT [dbo].[TraceIndentString](@TraceLevel,2) + N'Deleting Extension {' + CAST(@ExtensionId AS NVARCHAR(MAX)) + '}'
    IF EXISTS(SELECT * FROM [dbo].[AuthorityKeyIdentifier] [a] WHERE [a].[ExtensionId]=@ExtensionId)
    BEGIN
      EXECUTE [dbo].[DeleteAuthorityKeyIdentifier] @ExtensionId=@ExtensionId,@TraceLevel=@InnerTraceLevel
    END
    ELSE IF EXISTS(SELECT * FROM [dbo].[SubjectKeyIdentifier] [a] WHERE [a].[ExtensionId]=@ExtensionId)
    BEGIN
      EXECUTE [dbo].[DeleteSubjectKeyIdentifier] @ExtensionId=@ExtensionId,@TraceLevel=@InnerTraceLevel
    END
    DELETE FROM [dbo].[Extension] WHERE [ExtensionId]=@ExtensionId
  END
END