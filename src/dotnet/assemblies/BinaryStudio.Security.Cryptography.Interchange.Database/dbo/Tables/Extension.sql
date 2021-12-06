CREATE TABLE [dbo].[Extension] (
    [ExtensionId] INT IDENTITY (1, 1) NOT NULL,
    [ObjectId]    INT NULL,
    [Type]        INT NOT NULL,
    [IsCritical]  BIT NOT NULL,
    CONSTRAINT [CertificateExtension_PK] PRIMARY KEY CLUSTERED ([ExtensionId] ASC),
    CONSTRAINT [Extension_Object_FK] FOREIGN KEY ([ObjectId]) REFERENCES [dbo].[Object] ([ObjectId]),
    CONSTRAINT [Extension_ObjectIdentifier_FK] FOREIGN KEY ([Type]) REFERENCES [dbo].[ObjectIdentifier] ([Id])
);



