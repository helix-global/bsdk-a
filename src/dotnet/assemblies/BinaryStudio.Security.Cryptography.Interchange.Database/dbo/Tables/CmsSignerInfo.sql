CREATE TABLE [dbo].[CmsSignerInfo]
(
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [MessageId]   INT  NOT NULL,
    [Issuer]       INT          NULL,
    [IssuerSerialNumber] VARCHAR(50) NULL,
    [SignatureAlgorithm] INT NULL,
    [HashAlgorithm] INT NULL,
    [SigningTime] DATETIME NULL,
    CONSTRAINT [CmsSignerInfo_PK] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CmsSignerInfo_Issuer_FK] FOREIGN KEY ([Issuer]) REFERENCES [dbo].[RelativeDistinguishedNameSequence] ([RelativeDistinguishedNameSequenceId]),
    CONSTRAINT [CmsSignerInfo_SignatureAlgorithm_FK] FOREIGN KEY ([SignatureAlgorithm]) REFERENCES [dbo].[ObjectIdentifier] ([Id]),
    CONSTRAINT [CmsSignerInfo_HashAlgorithm_FK] FOREIGN KEY ([HashAlgorithm]) REFERENCES [dbo].[ObjectIdentifier] ([Id]),
    CONSTRAINT [CmsSignerInfo_Message_FK] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[CmsMessage] ([ObjectId])
)
