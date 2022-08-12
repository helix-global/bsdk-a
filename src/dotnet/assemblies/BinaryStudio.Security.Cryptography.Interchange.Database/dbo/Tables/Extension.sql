CREATE TABLE [dbo].[Extension] (
    [ExtensionId] INT IDENTITY (1, 1) NOT NULL,
    [ObjectId]    INT NULL,
    [Type]        INT NOT NULL,
    [IsCritical]  BIT NOT NULL,
    CONSTRAINT [CertificateExtension_PK] PRIMARY KEY CLUSTERED ([ExtensionId] ASC),
    CONSTRAINT [Extension_Object_FK] FOREIGN KEY ([ObjectId]) REFERENCES [dbo].[Object] ([ObjectId]),
    CONSTRAINT [Extension_ObjectIdentifier_FK] FOREIGN KEY ([Type]) REFERENCES [dbo].[ObjectIdentifier] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [_dta_index_Extension_47_101575400__K2]
    ON [dbo].[Extension]([ObjectId] ASC);


GO
CREATE STATISTICS [_dta_stat_101575400_2_1]
    ON [dbo].[Extension]([ObjectId], [ExtensionId]);

