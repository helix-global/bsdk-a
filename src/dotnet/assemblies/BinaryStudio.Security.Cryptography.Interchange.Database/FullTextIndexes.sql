CREATE FULLTEXT INDEX ON [dbo].[String]
    ([Value] LANGUAGE 1049)
    KEY INDEX [Strings_PK]
    ON [FTI];


GO
CREATE FULLTEXT INDEX ON [dbo].[InputData]
    ([RegisterNumber] LANGUAGE 1049)
    KEY INDEX [InputData_PK]
    ON [FTI];

