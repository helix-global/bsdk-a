CREATE TABLE [dbo].[Object] (
    [ObjectId]            INT            IDENTITY (1, 1) NOT NULL,
    [Type]                TINYINT        NOT NULL,
    [Body]                XML            NULL,
    [Group]               NVARCHAR (50)  NULL,
    [Key]                 NVARCHAR (MAX) NULL,
    [ShortFileIdentifier] VARCHAR (50)   NULL,
    CONSTRAINT [Object_PK] PRIMARY KEY CLUSTERED ([ObjectId] ASC)
);














GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20220422-183610]
    ON [dbo].[Object]([ObjectId] ASC)
    INCLUDE([Key]);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20220426-170555]
    ON [dbo].[Object]([ObjectId] ASC)
    INCLUDE([Group], [Key]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Object_47_197575742__K4_K1_5]
    ON [dbo].[Object]([Group] ASC, [ObjectId] ASC)
    INCLUDE([Key]);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20220602-105022]
    ON [dbo].[Object]([ObjectId] ASC)
    INCLUDE([Type], [Group], [Key], [ShortFileIdentifier]);


GO
CREATE STATISTICS [_dta_stat_197575742_4_6_2]
    ON [dbo].[Object]([Group], [ShortFileIdentifier], [Type]);


GO
CREATE STATISTICS [_dta_stat_197575742_4_6_1]
    ON [dbo].[Object]([Group], [ShortFileIdentifier], [ObjectId]);


GO
CREATE STATISTICS [_dta_stat_197575742_2_4]
    ON [dbo].[Object]([Type], [Group]);


GO
CREATE STATISTICS [_dta_stat_197575742_2_1_4_6]
    ON [dbo].[Object]([Type], [ObjectId], [Group], [ShortFileIdentifier]);

