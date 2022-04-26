CREATE TABLE [dbo].[Object] (
    [ObjectId] INT            IDENTITY (1, 1) NOT NULL,
    [Type]     TINYINT        NOT NULL,
    [Body]     XML            NULL,
    [Group]    TINYINT        NULL,
    [Key]      NVARCHAR (MAX) NULL,
    CONSTRAINT [Object_PK] PRIMARY KEY CLUSTERED ([ObjectId] ASC)
);








GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20220422-183610]
    ON [dbo].[Object]([ObjectId] ASC)
    INCLUDE([Key]);

