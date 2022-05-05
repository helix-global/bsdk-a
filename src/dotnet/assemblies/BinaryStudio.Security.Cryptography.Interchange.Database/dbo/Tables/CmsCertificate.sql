CREATE TABLE [dbo].[CmsCertificate] (
    [MessageId]     INT NOT NULL,
    [CertificateId] INT NOT NULL,
    CONSTRAINT [CmsCertificate_Certificate_FK] FOREIGN KEY ([CertificateId]) REFERENCES [dbo].[Certificate] ([ObjectId]),
    CONSTRAINT [CmsCertificate_CmsMessage_FK] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[CmsMessage] ([ObjectId])
);



GO
CREATE NONCLUSTERED INDEX [_dta_index_CmsCertificate_47_886346272__K1_2]
    ON [dbo].[CmsCertificate]([MessageId] ASC)
    INCLUDE([CertificateId]);

