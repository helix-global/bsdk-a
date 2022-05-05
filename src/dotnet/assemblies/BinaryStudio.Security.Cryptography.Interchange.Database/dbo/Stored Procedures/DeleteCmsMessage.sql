-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [DeleteCmsMessage]
  @ObjectId INT
AS
BEGIN
	SET NOCOUNT ON;
  IF @ObjectId IS NOT NULL
  BEGIN
    BEGIN TRANSACTION
      DELETE FROM [dbo].[CmsCertificate] WHERE [MessageId]=@ObjectId
      DELETE FROM [dbo].[CmsSignerInfo]  WHERE [MessageId]=@ObjectId
      DELETE FROM [dbo].[CmsMessage] WHERE [ObjectId]=@ObjectId
      DELETE FROM [dbo].[Object]     WHERE [ObjectId]=@ObjectId
    COMMIT
  END
END