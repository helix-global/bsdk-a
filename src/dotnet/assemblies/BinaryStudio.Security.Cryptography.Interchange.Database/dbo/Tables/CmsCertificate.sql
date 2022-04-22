CREATE TABLE [dbo].[CmsCertificate]
(
    [MessageId]   INT  NOT NULL,
    [CertificateId]  INT  NOT NULL,
    CONSTRAINT [CmsCertificate_CmsMessage_FK] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[CmsMessage] ([ObjectId]),
    CONSTRAINT [CmsCertificate_Certificate_FK] FOREIGN KEY ([CertificateId]) REFERENCES [dbo].[Certificate] ([ObjectId]),
)
