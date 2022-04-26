







CREATE VIEW [dbo].[ReportFailed]
AS
SELECT
   [a].*
FROM [dbo].[InputData] [a] WITH(NOLOCK)
  INNER JOIN [dbo].[Failed] [b] ON ([b].[Key]=[a].[IdentifyDocumentId]) AND ([a].[Group]=[b].[Group])