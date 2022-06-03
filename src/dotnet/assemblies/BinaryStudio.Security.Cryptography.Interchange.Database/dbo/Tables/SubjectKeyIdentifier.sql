CREATE TABLE [dbo].[SubjectKeyIdentifier] (
    [ExtensionId] INT           NOT NULL,
    [LongKey]     VARCHAR (128) NOT NULL,
    [ShortKey]    CHAR (4)      NOT NULL,
    CONSTRAINT [SubjectKeyIdentifier_PK] PRIMARY KEY CLUSTERED ([ExtensionId] ASC),
    CONSTRAINT [SubjectKeyIdentifier_Extension_FK] FOREIGN KEY ([ExtensionId]) REFERENCES [dbo].[Extension] ([ExtensionId]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [_dta_index_SubjectKeyIdentifier_47_421576540__K1_3]
    ON [dbo].[SubjectKeyIdentifier]([ExtensionId] ASC)
    INCLUDE([ShortKey]);

