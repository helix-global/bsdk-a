CREATE TABLE [dbo].[ObjectIdentifier] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Value]     VARCHAR (128)  NOT NULL,
    [ShortName] NVARCHAR (MAX) NULL,
    CONSTRAINT [ObjectIdentifierT_PK] PRIMARY KEY CLUSTERED ([Id] ASC)
);












GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20211116-162514]
    ON [dbo].[ObjectIdentifier]([Value] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20220422-184611]
    ON [dbo].[ObjectIdentifier]([Id] ASC)
    INCLUDE([Value]);

