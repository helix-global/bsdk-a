CREATE TABLE [dbo].[Failed] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [Key]   NVARCHAR (255) NULL,
    [Group] INT            NOT NULL,
    CONSTRAINT [Failed_PK] PRIMARY KEY CLUSTERED ([Id] ASC)
);



