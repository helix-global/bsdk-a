CREATE TABLE [dbo].[Certificate] (
    [ObjectId]     INT          NOT NULL,
    [Country]      VARCHAR (5)  NULL,
    [SerialNumber] VARCHAR (50) NOT NULL,
    [Thumbprint]   VARCHAR (64) NOT NULL,
    [Issuer]       INT          NOT NULL,
    [Subject]      INT          NOT NULL,
    [NotBefore]    DATETIME     NOT NULL,
    [NotAfter]     DATETIME     NULL,
    [SignatureAlgorithm] INT NULL,
    [HashAlgorithm] INT NULL, 
    CONSTRAINT [Certificate_PK] PRIMARY KEY CLUSTERED ([ObjectId] ASC),
    CONSTRAINT [Certificate_Object_FK] FOREIGN KEY ([ObjectId]) REFERENCES [dbo].[Object] ([ObjectId]) ON DELETE CASCADE,
    CONSTRAINT [Certificate_RelativeDistinguishedNameSequence_FK] FOREIGN KEY ([Issuer]) REFERENCES [dbo].[RelativeDistinguishedNameSequence] ([RelativeDistinguishedNameSequenceId]),
    CONSTRAINT [Certificate_RelativeDistinguishedNameSequence_FK1] FOREIGN KEY ([Subject]) REFERENCES [dbo].[RelativeDistinguishedNameSequence] ([RelativeDistinguishedNameSequenceId]),
    CONSTRAINT [Certificate_SignatureAlgorithm_ObjectIdentifier_FK] FOREIGN KEY ([SignatureAlgorithm]) REFERENCES [dbo].[ObjectIdentifier] ([Id]),
    CONSTRAINT [Certificate_HashAlgorithm_ObjectIdentifier_FK] FOREIGN KEY ([HashAlgorithm]) REFERENCES [dbo].[ObjectIdentifier] ([Id])
);














GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20211115-200045]
    ON [dbo].[Certificate]([Thumbprint] ASC);

