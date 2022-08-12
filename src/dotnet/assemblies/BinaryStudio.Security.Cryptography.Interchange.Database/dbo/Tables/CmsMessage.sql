CREATE TABLE [dbo].[CmsMessage] (
    [ObjectId]      INT          NOT NULL,
    [ContentType]   INT          NULL,
    [HashAlgorithm] INT          NULL,
    [Thumbprint]    VARCHAR (64) NULL,
    CONSTRAINT [CmsMessage_PK] PRIMARY KEY CLUSTERED ([ObjectId] ASC),
    CONSTRAINT [CmsMessage_ContentType_FK] FOREIGN KEY ([ContentType]) REFERENCES [dbo].[ObjectIdentifier] ([Id]),
    CONSTRAINT [CmsMessage_HashAlgorithm_FK] FOREIGN KEY ([HashAlgorithm]) REFERENCES [dbo].[ObjectIdentifier] ([Id]),
    CONSTRAINT [CmsMessage_Object_FK] FOREIGN KEY ([ObjectId]) REFERENCES [dbo].[Object] ([ObjectId]) ON DELETE CASCADE
);







GO
CREATE STATISTICS [_dta_stat_902346329_1_3]
    ON [dbo].[CmsMessage]([ObjectId], [HashAlgorithm]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_CmsMessage_47_902346329__K1]
    ON [dbo].[CmsMessage]([ObjectId] ASC);

