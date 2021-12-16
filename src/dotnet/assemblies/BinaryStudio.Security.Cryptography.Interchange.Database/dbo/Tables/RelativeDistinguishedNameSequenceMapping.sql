CREATE TABLE [dbo].[RelativeDistinguishedNameSequenceMapping] (
    [MappingId]                           INT IDENTITY (1, 1) NOT NULL,
    [RelativeDistinguishedNameSequenceId] INT NOT NULL,
    [RelativeDistinguishedNameId]         INT NOT NULL,
    CONSTRAINT [RelativeDistinguishedNameSequenceMapping_PK] PRIMARY KEY CLUSTERED ([MappingId] ASC),
    CONSTRAINT [RelativeDistinguishedNameSequenceMapping_RelativeDistinguishedName_FK] FOREIGN KEY ([RelativeDistinguishedNameId]) REFERENCES [dbo].[RelativeDistinguishedName] ([RelativeDistinguishedNameId]),
    CONSTRAINT [RelativeDistinguishedNameSequenceMapping_RelativeDistinguishedNameSequence_FK] FOREIGN KEY ([RelativeDistinguishedNameSequenceId]) REFERENCES [dbo].[RelativeDistinguishedNameSequence] ([RelativeDistinguishedNameSequenceId])
);



