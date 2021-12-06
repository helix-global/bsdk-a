CREATE TABLE [dbo].[String] (
    [Id]    INT            IDENTITY (1, 1) NOT NULL,
    [Value] NVARCHAR (MAX) NULL,
    CONSTRAINT [Strings_PK] PRIMARY KEY CLUSTERED ([Id] ASC)
);

