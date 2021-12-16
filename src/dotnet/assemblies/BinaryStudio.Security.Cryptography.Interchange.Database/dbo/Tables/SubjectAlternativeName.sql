CREATE TABLE [dbo].[SubjectAlternativeName] (
    [ExtensionId] INT NOT NULL,
    CONSTRAINT [SubjectAlternativeName_PK] PRIMARY KEY CLUSTERED ([ExtensionId] ASC),
    CONSTRAINT [SubjectAlternativeName_Extension_FK] FOREIGN KEY ([ExtensionId]) REFERENCES [dbo].[Extension] ([ExtensionId]) ON DELETE CASCADE
);

