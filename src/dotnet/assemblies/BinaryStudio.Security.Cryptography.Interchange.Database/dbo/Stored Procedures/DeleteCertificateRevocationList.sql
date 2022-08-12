-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteCertificateRevocationList]
  @ObjectId INT,
  @TraceLevel INT = 0
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @InnerTraceLevel INT = @TraceLevel+1
  IF @ObjectId IS NOT NULL
  BEGIN
    PRINT [dbo].[TraceIndentString](@TraceLevel,2) + N'Deleting Crl {' + CAST(@ObjectId AS NVARCHAR(MAX)) + '}'
    DECLARE @ExtensionId INT
    DECLARE [c] CURSOR LOCAL FOR
      SELECT
        [a].[ExtensionId]
      FROM [dbo].[Extension] [a]
      WHERE ([a].[ObjectId]=@ObjectId)
    OPEN [c]
    FETCH NEXT FROM [c] INTO @ExtensionId
    WHILE @@FETCH_STATUS = 0
    BEGIN
      EXECUTE [dbo].[DeleteExtension] @ExtensionId=@ExtensionId,@TraceLevel=@InnerTraceLevel
      FETCH NEXT FROM [c] INTO @ExtensionId
    END
    CLOSE [c]
    DEALLOCATE [c]
    DELETE FROM [dbo].[CertificateRevocationList] WHERE [ObjectId]=@ObjectId
    DELETE FROM [dbo].[Object] WHERE [ObjectId]=@ObjectId
  END
END