CREATE TABLE [dbo].[InputData] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [IdentifyDocumentId]   VARCHAR (50)  NULL,
    [DocumentCategoryName] NVARCHAR (50) NULL,
    [CountryName]          NVARCHAR (50) NULL,
    [CountryICAO]          NVARCHAR (50) NULL,
    [Year]                 VARCHAR (50)  NULL,
    [Month]                VARCHAR (50)  NULL,
    [RegisterCode]         VARCHAR (50)  NULL,
    [RegisterNumber]       VARCHAR (50)  NULL,
    [IssueDate]            VARCHAR (50)  NULL,
    [ValidToDate]          VARCHAR (50)  NULL,
    [InscribeId]           VARCHAR (50)  NULL,
    [DataFormatId]         VARCHAR (50)  NULL,
    [DataTypeId]           VARCHAR (50)  NULL,
    [Order]                VARCHAR (50)  NULL,
    [Dense]                VARCHAR (50)  NULL,
    [Size]                 INT           NULL,
    [Group]                TINYINT       NULL,
    CONSTRAINT [InputData_PK] PRIMARY KEY CLUSTERED ([Id] ASC)
);










GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20220426-173317]
    ON [dbo].[InputData]([Id] ASC)
    INCLUDE([RegisterNumber]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_InputData_47_1926402032__K9_K2_K1_K18]
    ON [dbo].[InputData]([RegisterNumber] ASC, [IdentifyDocumentId] ASC, [Id] ASC, [Group] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_InputData_47_1926402032__K9_K2]
    ON [dbo].[InputData]([RegisterNumber] ASC, [IdentifyDocumentId] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_InputData_47_1926402032__K2_K9]
    ON [dbo].[InputData]([IdentifyDocumentId] ASC, [RegisterNumber] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_InputData_47_1926402032__K2_K1_K9_K18]
    ON [dbo].[InputData]([IdentifyDocumentId] ASC, [Id] ASC, [RegisterNumber] ASC, [Group] ASC);


GO
CREATE STATISTICS [_dta_stat_1926402032_2_18]
    ON [dbo].[InputData]([IdentifyDocumentId], [Group]);


GO
CREATE STATISTICS [_dta_stat_1926402032_18_1_2]
    ON [dbo].[InputData]([Group], [Id], [IdentifyDocumentId]);

