CREATE TABLE [dbo].[AuthorityKeyIdentifier] (
    [ExtensionId]  INT           NOT NULL,
    [LongKey]      VARCHAR (128) NOT NULL,
    [ShortKey]     CHAR (4)      NOT NULL,
    [SerialNumber] VARCHAR (128) NULL,
    [Issuer]       INT           NULL,
    CONSTRAINT [AuthorityKeyIdentifier_PK] PRIMARY KEY CLUSTERED ([ExtensionId] ASC),
    CONSTRAINT [AuthorityKeyIdentifier_CertificateExtension_FK] FOREIGN KEY ([ExtensionId]) REFERENCES [dbo].[Extension] ([ExtensionId]) ON DELETE CASCADE,
    CONSTRAINT [AuthorityKeyIdentifier_GeneralName_FK] FOREIGN KEY ([Issuer]) REFERENCES [dbo].[GeneralName] ([GeneralNameId])
);






GO
CREATE NONCLUSTERED INDEX [_dta_index_AuthorityKeyIdentifier_47_5575058__K1_3]
    ON [dbo].[AuthorityKeyIdentifier]([ExtensionId] ASC)
    INCLUDE([ShortKey]);


GO
CREATE STATISTICS [_dta_stat_5575058_3_1]
    ON [dbo].[AuthorityKeyIdentifier]([ShortKey], [ExtensionId]);

