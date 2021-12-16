CREATE TABLE [dbo].[RelativeDistinguishedNameSequence] (
    [RelativeDistinguishedNameSequenceId] INT NOT NULL,
    CONSTRAINT [X509RelativeDistinguishedNameSequence_PK] PRIMARY KEY CLUSTERED ([RelativeDistinguishedNameSequenceId] ASC),
    CONSTRAINT [RelativeDistinguishedNameSequence_GeneralName_FK] FOREIGN KEY ([RelativeDistinguishedNameSequenceId]) REFERENCES [dbo].[GeneralName] ([GeneralNameId]) ON DELETE CASCADE
);





