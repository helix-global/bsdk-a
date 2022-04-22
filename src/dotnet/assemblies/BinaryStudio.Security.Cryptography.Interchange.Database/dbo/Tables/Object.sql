CREATE TABLE [dbo].[Object] (
    [ObjectId] INT     IDENTITY (1, 1) NOT NULL,
    [Type]     TINYINT NOT NULL,
    [Body]     XML     NULL,
    [Status] TINYINT NULL, 
    [Key] NVARCHAR(MAX) NULL, 
    CONSTRAINT [Object_PK] PRIMARY KEY CLUSTERED ([ObjectId] ASC)
);



