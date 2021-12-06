CREATE TABLE [dbo].[IssuerAlternativeName] (
    [ExtensionId] INT NOT NULL,
    CONSTRAINT [IssuerAlternativeName_PK] PRIMARY KEY CLUSTERED ([ExtensionId] ASC),
    CONSTRAINT [IssuerAlternativeName_Extension_FK] FOREIGN KEY ([ExtensionId]) REFERENCES [dbo].[Extension] ([ExtensionId]) ON DELETE CASCADE
);

