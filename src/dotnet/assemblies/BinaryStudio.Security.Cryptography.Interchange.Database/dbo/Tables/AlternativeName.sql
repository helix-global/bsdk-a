CREATE TABLE [dbo].[AlternativeName] (
    [MappingId]     INT IDENTITY (1, 1) NOT NULL,
    [ExtensionId]   INT NULL,
    [GeneralNameId] INT NULL,
    CONSTRAINT [AlternativeName_PK] PRIMARY KEY CLUSTERED ([MappingId] ASC),
    CONSTRAINT [AlternativeName_Extension_FK] FOREIGN KEY ([ExtensionId]) REFERENCES [dbo].[Extension] ([ExtensionId]),
    CONSTRAINT [AlternativeName_GeneralName_FK] FOREIGN KEY ([GeneralNameId]) REFERENCES [dbo].[GeneralName] ([GeneralNameId])
);



