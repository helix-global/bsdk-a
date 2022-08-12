-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ReportG3]
  @SourceTable NVARCHAR(MAX)
AS
BEGIN
  SET NOCOUNT ON;
  IF OBJECT_ID('tempdb..##ReportSource') IS NOT NULL
    DROP TABLE ##ReportSource
  IF OBJECT_ID('tempdb..##ReportTarget') IS NOT NULL
    DROP TABLE ##ReportTarget
  DECLARE @Sql NVARCHAR(MAX) = N'SELECT * INTO ##ReportSource FROM ' + @SourceTable
  EXECUTE sp_executesql @Sql
  SELECT DISTINCT
       [a].[Id]
      ,[a].[IdentifyDocumentId]
      ,[a].[ShortFileIdentifier]
      ,[a].[DocumentCategoryName]
      ,[a].[CountryName]
      ,CASE [a].[CountryICAO]
        WHEN 'D' THEN 'deu'
        ELSE [a].[CountryICAO]
        END [CountryICAO]
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
        WHEN '00000000' THEN
          CASE
            WHEN [b].[Flags]='2' THEN '09. Нет (Проверка сертификата)'
            WHEN [b].[Flags]='4' THEN 'Нет (Проверка списка отзыва)'
          ELSE '12. Нет (Проверка сообщения)' END
        WHEN '0' THEN
          CASE
            WHEN [b].[Flags]='2' THEN '09. Нет (Проверка сертификата)'
            WHEN [b].[Flags]='4' THEN 'Нет (Проверка списка отзыва)'
          ELSE '12. Нет (Проверка сообщения)' END
        WHEN 'd4460009' THEN '05. Не удалось найти список отозванных сертификатов'
        WHEN 'd4460006' THEN '04. Цепочка сертификатов не завершена'
        WHEN 'd000a000' THEN
          CASE
            WHEN [b].[Flags]='2' THEN '03. Проверка сигнатуры сертификата'
            WHEN [b].[Flags]='4' THEN '07. Проверка сигнатуры списка отзыва'
          ELSE '11. Ошибка проверка сигнатуры сообщения' END
        WHEN '80090006' THEN
          CASE
            WHEN [b].[Flags]='2' THEN '03. Проверка сигнатуры сертификата'
            WHEN [b].[Flags]='4' THEN '07. Проверка сигнатуры списка отзыва'
          ELSE '11. Ошибка проверка сигнатуры сообщения' END
        WHEN 'NULL'     THEN '01. Не удалось декодировать EFSOD'
        ELSE ISNULL(CASE
          WHEN [b].[Source] LIKE '%CCryptVerifyCertificate%'                                    THEN '08. Техническая ошибка проверки сертификата'
          WHEN [b].[Source] LIKE 'Истек срок действия сертификата'                              THEN '02. Истек срок действия сертификата'
          WHEN [b].[Source] LIKE 'Импорт ключа'                                                 THEN '10. Ошибка импорта ключа для проверки сообщения'
          WHEN [b].[Source] LIKE 'Не удается проверить подпись списка отозванных сертификатов.' THEN '06. Не удается проверить подпись списка отозванных сертификатов'
          WHEN [b].[Source] LIKE 'Ошибка проверки сигнатуры сертификата.'                       THEN '03. Проверка сигнатуры сертификата'
          WHEN [b].[Source] LIKE 'Проверка сигнатуры' THEN
            CASE
              WHEN [b].[Flags]='2' THEN '03. Проверка сигнатуры сертификата'
              WHEN [b].[Flags]='4' THEN '07. Проверка сигнатуры списка отзыва'
            ELSE '11. Ошибка проверка сигнатуры сообщения' END
          ELSE [b].[Source]
          END,[h].[SCodeMessage])
        END [Источник ошибки]
      ,[dbo].[OidToStr]([b].[ActualContentDigestMethod]) [Используемый aлгоритм хэширования]
      ,NULLIF([b].[CCryptError],'NULL') [Статус CRYPTAPI]
      ,NULLIF([b].[BCryptError],'NULL') [Статус BCRYPT]
      ,NULLIF([b].[Modifiers],'NULL') [Модификаторы]
      ,[f].[NotBefore] [Начальная дата периода действия сертификата]
      ,[f].[NotAfter] [Конечная дата периода действия сертификата]
      ,[c].[SigningTime] [Дата подписи]
      ,[f].[Issuer] [Издатель]
      ,[f].[Subject] [Субъект]
      ,NULLIF([f].[AuthorityKeyIdentifier],'NULL') [УЦ]
      ,[b].[Certificate] [Сертификат]
      ,NULLIF([b].[Crl],'NULL') [Список отзыва]
      ,[b].[Stream] [Исходный файл]
      ,[b].[Scode]
      ,CASE [b].[Flags]
        WHEN '1' THEN 'Проверка сообщения'
        WHEN '2' THEN 'Проверка сертификата'
        WHEN '4' THEN 'Проверка списка отзыва'
        ELSE 'Декодирование EFSOD'
        END [Категория]
  INTO ##ReportTarget
  FROM [dbo].[InputData] [a]
    LEFT JOIN
    (
      SELECT DISTINCT * FROM ##ReportSource
    ) [b] ON [b].[Stream]=([a].[IdentifyDocumentId]+'{' + [a].[ShortFileIdentifier]+ '}')
    LEFT JOIN [dbo].[ReportMessage] [c] ON ([c].[Key]=[a].[IdentifyDocumentId]) AND ([c].[Group]=[a].[Group]) AND ([c].[ShortFileIdentifier]=[a].[ShortFileIdentifier])
    LEFT JOIN [dbo].[ThreeLetterISOCountryCode] [d] ON [d].[ThreeLetterISOCountryCode]=[a].[CountryICAO]
    LEFT JOIN [dbo].[Country] [e] ON [e].[Country]=[d].[Country]
    LEFT JOIN [dbo].[ReportCertificate] [f] ON [f].[ObjectId]=[c].[Certificate]
    LEFT JOIN [dbo].[SCode] [h] ON [h].[SCode]=[b].[SCode]
  WHERE ([a].[Group]='шумилкино{доп}')
    AND ([a].[Size] IS NOT NULL)
    --AND ([f].[NotAfter]>=GETDATE())
    --AND ([a].[ShortFileIdentifier]='1d')
    --AND [e].[Country]='af'
    --AND ([a].[Year]='2010')
    --AND ([a].[Stream] LIKE '0000004607%')
  --and ([b].[Scode] IS NULL)
  ORDER BY [b].[Stream]

  SELECT *
  FROM ##ReportTarget [a]
  WHERE ((([a].[Страна(Код)]='ru') AND ([a].[ShortFileIdentifier]='e040')) OR ([a].[Страна(Код)]<>'ru'))
    AND (CAST([a].[ValidToDate] as DATETIME) >= '2022-05-22T00:00:00')
    AND ([a].[Источник ошибки] <> 'Ошибка проверки сроков действия сертификата.')
  ORDER BY [IdentifyDocumentId],[ShortFileIdentifier]
END