



/****** Скрипт для команды SelectTopNRows из среды SSMS  ******/
CREATE VIEW [dbo].[DecodingFailedInGroup1SucceededInSameGroup]
AS
SELECT
   [b].[RegisterNumber] [RegisterNumberInGroup1]
  ,[a].[Key] [FailedDocumentInGroup1]
  ,[c].[IdentifyDocumentId] [SucceededDocumentInGroup1]
  ,[e].*
FROM [dbo].[DecodingFailed] [a]
  INNER JOIN [dbo].[InputData] [b] ON ([b].[IdentifyDocumentId]=[a].[Key]) AND ([b].[Group]=[a].[Group])
  INNER JOIN [dbo].[InputData] [c] ON ([c].[RegisterNumber]=[b].[RegisterNumber]) AND ([c].[Group] = [b].[Group]) AND ([c].[IdentifyDocumentId]<>[b].[IdentifyDocumentId])
  INNER JOIN [dbo].[Object]    [d] ON ([d].[Key]=[c].[IdentifyDocumentId]) AND ([d].[Group]=[c].[Group])
  INNER JOIN [dbo].[ReportMessage] [e] ON [e].[ObjectId]=[d].[ObjectId]
WHERE ([a].[Group]=1)