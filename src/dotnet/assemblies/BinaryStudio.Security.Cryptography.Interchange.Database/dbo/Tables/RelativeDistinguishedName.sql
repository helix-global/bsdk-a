CREATE TABLE [dbo].[RelativeDistinguishedName] (
    [RelativeDistinguishedNameId] INT IDENTITY (1, 1) NOT NULL,
    [Type]                        INT NOT NULL,
    [Value]                       INT NOT NULL,
    CONSTRAINT [X509RelativeDistinguishedName_PK] PRIMARY KEY CLUSTERED ([RelativeDistinguishedNameId] ASC),
    CONSTRAINT [RelativeDistinguishedName_Strings_FK] FOREIGN KEY ([Value]) REFERENCES [dbo].[String] ([Id]),
    CONSTRAINT [RelativeDistinguishedName_TObjectIdentifier_FK] FOREIGN KEY ([Type]) REFERENCES [dbo].[ObjectIdentifier] ([Id])
);







