-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ReportG3]
AS
BEGIN
  SET NOCOUNT ON;
  SELECT
         [a].[IdentifyDocumentId]
        ,[a].[DocumentCategoryName]
        ,[a].[CountryName]
        ,[a].[CountryICAO]
        ,[a].[Year]
        ,[a].[Month]
        ,[a].[RegisterCode]
        ,[a].[RegisterNumber]
        ,[a].[IssueDate]
        ,[a].[ValidToDate]
        ,[a].[InscribeId]
        ,[a].[DataFormatId]
        ,[a].[DataTypeId]
        ,[a].[Order]
        ,[a].[Dense]
        ,[a].[Size]
        ,CASE ISNULL([b].[SCode],'NULL')
          WHEN '00000000' THEN 'False'
          WHEN 'NULL'     THEN 'True'
          ELSE 'True'
         END [Ошибка]
        ,[c].[SignerSignatureAlgorithm] [Алгоритм подписи(Сообщение)]
        ,[c].[SignerHashAlgorithm] [Алгоритм хэширования(Сообщение)]
        ,[e].[Country] [Страна(Код)]
        ,[e].[RussianShortName] [Страна(Наименование)]
        ,[f].[SignatureAlgorithm] [Алгоритм подписи(Сертификат)]
        ,[f].[HashAlgorithm] [Алгоритм хэширования(Сертификат)]
        ,[b].[Organization] [Организация]
        ,CASE ISNULL([b].[SCode],'NULL')
          WHEN '00000000' THEN CASE WHEN [b].[MessageSize]='0' THEN 'Нет (Проверка сертификата)' ELSE 'Нет (Проверка сообщения)' END
          WHEN 'd4460009' THEN 'Не удалось найти список отозванных сертификатов'
          WHEN 'd4460006' THEN 'Цепочка сертификатов не завершена'
          WHEN 'NULL'     THEN 'Не удалось декодировать EFSOD'
          ELSE ISNULL(CASE WHEN [b].[Source] LIKE '%CCryptVerifyCertificate%' THEN 'Ошибка проверки сертификата' ELSE [b].[Source] END,[h].[SCodeMessage])
         END [Источник ошибки]
        ,[dbo].[OidToStr]([b].[ActualContentDigestMethod]) [Используемый aлгоритм хэширования]
        ,[b].[CCryptError] [Статус CRYPTAPI]
        ,[b].[BCryptError] [Статус BCRYPT]
        ,[b].[Modifiers] [Модификаторы]
        ,[f].[NotBefore] [Начальная дата периода действия сертификата]
        ,[f].[NotAfter] [Конечная дата периода действия сертификата]
        ,[c].[SigningTime] [Дата подписи]
        ,[f].[Issuer] [Издатель]
        ,[f].[Subject] [Субъект]
        ,[f].[AuthorityKeyIdentifier] [УЦ]
        ,[b].[Certificate] [Сертификат]
        ,[b].[Crl] [Список отзыва]
        ,[b].[Stream] [Исходный файл]
        ,[b].[Scode]
  FROM [dbo].[InputData] [a]
    LEFT JOIN
    (
      SELECT DISTINCT * FROM [p0] UNION ALL
      SELECT DISTINCT * FROM [p1] UNION ALL
      SELECT DISTINCT * FROM [p2]
    ) [b] ON [b].[Stream]=([a].[IdentifyDocumentId]+'{' + [a].[ShortFileIdentifier]+ '}')
    LEFT JOIN [dbo].[ReportMessage] [c] ON [c].[Key]=[a].[IdentifyDocumentId]
    LEFT JOIN [dbo].[ThreeLetterISOCountryCode] [d] ON [d].[ThreeLetterISOCountryCode]=[a].[CountryICAO]
    LEFT JOIN [dbo].[Country] [e] ON [e].[Country]=[d].[Country]
    LEFT JOIN [dbo].[ReportCertificate] [f] ON [f].[ObjectId]=[c].[Certificate]
    LEFT JOIN [dbo].[SCode] [h] ON [h].[SCode]=[b].[SCode]
  WHERE ([a].[Group]=3)
    AND ([a].[ShortFileIdentifier]='1d')
    --AND [e].[Country]='af'
  --and ([a].[IdentifyDocumentId]='000000095c')
  ORDER BY [a].[IdentifyDocumentId]
END