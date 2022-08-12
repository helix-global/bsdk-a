CREATE TABLE [dbo].[CmsSignerInfo] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [MessageId]          INT          NOT NULL,
    [Issuer]             INT          NULL,
    [IssuerSerialNumber] VARCHAR (50) NULL,
    [SignatureAlgorithm] INT          NULL,
    [HashAlgorithm]      INT          NULL,
    [SigningTime]        DATETIME     NULL,
    CONSTRAINT [CmsSignerInfo_PK] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CmsSignerInfo_HashAlgorithm_FK] FOREIGN KEY ([HashAlgorithm]) REFERENCES [dbo].[ObjectIdentifier] ([Id]),
    CONSTRAINT [CmsSignerInfo_Issuer_FK] FOREIGN KEY ([Issuer]) REFERENCES [dbo].[RelativeDistinguishedNameSequence] ([RelativeDistinguishedNameSequenceId]),
    CONSTRAINT [CmsSignerInfo_Message_FK] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[CmsMessage] ([ObjectId]),
    CONSTRAINT [CmsSignerInfo_SignatureAlgorithm_FK] FOREIGN KEY ([SignatureAlgorithm]) REFERENCES [dbo].[ObjectIdentifier] ([Id])
);





GO
CREATE NONCLUSTERED INDEX [_dta_index_CmsSignerInfo_47_934346443__K2_K6_K5_K3_4_7]
    ON [dbo].[CmsSignerInfo]([MessageId] ASC, [HashAlgorithm] ASC, [SignatureAlgorithm] ASC, [Issuer] ASC)
    INCLUDE([IssuerSerialNumber], [SigningTime]);


GO
CREATE STATISTICS [_dta_stat_934346443_5_6]
    ON [dbo].[CmsSignerInfo]([SignatureAlgorithm], [HashAlgorithm]);


GO
CREATE STATISTICS [_dta_stat_934346443_3_2_6]
    ON [dbo].[CmsSignerInfo]([Issuer], [MessageId], [HashAlgorithm]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_CmsSignerInfo_47_934346443__K2_K6_K5_7]
    ON [dbo].[CmsSignerInfo]([MessageId] ASC, [HashAlgorithm] ASC, [SignatureAlgorithm] ASC)
    INCLUDE([SigningTime]);

