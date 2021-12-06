CREATE TABLE [dbo].[GeneralName] (
    [GeneralNameId] INT            IDENTITY (1, 1) NOT NULL,
    [Type]          TINYINT        NOT NULL,
    [Value]         NVARCHAR (MAX) NULL,
    CONSTRAINT [X509GeneralName_PK] PRIMARY KEY CLUSTERED ([GeneralNameId] ASC)
);



