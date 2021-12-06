CREATE TABLE [dbo].[CertificateRevocationList] (
    [ObjectId]      INT          NOT NULL,
    [Country]       VARCHAR (5)  NULL,
    [Issuer]        INT          NULL,
    [EffectiveDate] DATETIME     NOT NULL,
    [NextUpdate]    DATETIME     NULL,
    [Thumbprint]    VARCHAR (64) NOT NULL,
    CONSTRAINT [CRL_PK] PRIMARY KEY CLUSTERED ([ObjectId] ASC),
    CONSTRAINT [Crl_Object_FK] FOREIGN KEY ([ObjectId]) REFERENCES [dbo].[Object] ([ObjectId]) ON DELETE CASCADE,
    CONSTRAINT [Crl_RelativeDistinguishedNameSequence_FK] FOREIGN KEY ([Issuer]) REFERENCES [dbo].[RelativeDistinguishedNameSequence] ([RelativeDistinguishedNameSequenceId])
);

