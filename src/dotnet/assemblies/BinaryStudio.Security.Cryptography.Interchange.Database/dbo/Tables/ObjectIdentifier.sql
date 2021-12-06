CREATE TABLE [dbo].[ObjectIdentifier] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Value]     VARCHAR (128)  NOT NULL,
    [ShortName] NVARCHAR (MAX) NULL,
    CONSTRAINT [ObjectIdentifierT_PK] PRIMARY KEY CLUSTERED ([Id] ASC)
);










GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20211116-162514]
    ON [dbo].[ObjectIdentifier]([Value] ASC);

