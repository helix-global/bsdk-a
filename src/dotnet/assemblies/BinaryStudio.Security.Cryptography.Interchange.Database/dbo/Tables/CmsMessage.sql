﻿CREATE TABLE [dbo].[CmsMessage] (
    [ObjectId]      INT          NOT NULL,
    [ContentType]   INT          NULL,
    [HashAlgorithm] INT          NULL,
    [Thumbprint]    VARCHAR (64) NULL,
    CONSTRAINT [CmsMessage_PK] PRIMARY KEY CLUSTERED ([ObjectId] ASC),
    CONSTRAINT [CmsMessage_ContentType_FK] FOREIGN KEY ([ContentType]) REFERENCES [dbo].[ObjectIdentifier] ([Id]),
    CONSTRAINT [CmsMessage_HashAlgorithm_FK] FOREIGN KEY ([HashAlgorithm]) REFERENCES [dbo].[ObjectIdentifier] ([Id]),
    CONSTRAINT [CmsMessage_Object_FK] FOREIGN KEY ([ObjectId]) REFERENCES [dbo].[Object] ([ObjectId]) ON DELETE CASCADE
);


