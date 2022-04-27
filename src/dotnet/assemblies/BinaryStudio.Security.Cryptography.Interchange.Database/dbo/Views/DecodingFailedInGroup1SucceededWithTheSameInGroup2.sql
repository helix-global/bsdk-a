
/****** Скрипт для команды SelectTopNRows из среды SSMS  ******/
CREATE VIEW [dbo].[DecodingFailedInGroup1SucceededWithTheSameInGroup2]
AS
SELECT
   [b].[RegisterNumber] [RegisterNumberInGroup1]
  ,[a].[Key] [FailedDocumentInGroup1]
  ,[c].[IdentifyDocumentId] [TheSameDocumentInGroup2]
  ,[e].*
FROM [dbo].[DecodingFailed] [a]
  INNER JOIN [dbo].[InputData] [b] ON ([b].[IdentifyDocumentId]=[a].[Key]) AND ([b].[Group]=[a].[Group])
  INNER JOIN [dbo].[InputData] [c] ON ([c].[RegisterNumber]=[b].[RegisterNumber]) AND ([c].[Group] = 2)
  INNER JOIN [dbo].[Object]    [d] ON ([d].[Key]=[c].[IdentifyDocumentId]) AND ([d].[Group]=[c].[Group])
  INNER JOIN [dbo].[ReportMessage] [e] ON [e].[ObjectId]=[d].[ObjectId]
WHERE ([a].[Group]=1)