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
    [XmlSize]              INT           NULL,
    [Size]                 INT           NULL,
    [ShortFileIdentifier]  VARCHAR (50)  NULL,
    [Group]                NVARCHAR (50) NULL,
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


GO
CREATE NONCLUSTERED INDEX [_dta_index_InputData_47_1926402032__K18_2_3_4_5_6_7_8_9_10_11_12_13_14_15_16_17]
    ON [dbo].[InputData]([Group] ASC)
    INCLUDE([IdentifyDocumentId], [DocumentCategoryName], [CountryName], [CountryICAO], [Year], [Month], [RegisterCode], [RegisterNumber], [IssueDate], [ValidToDate], [InscribeId], [DataFormatId], [DataTypeId], [Order], [Dense], [Size]);


GO
CREATE STATISTICS [_dta_stat_1926402032_5_2]
    ON [dbo].[InputData]([CountryICAO], [IdentifyDocumentId]);


GO
CREATE STATISTICS [_dta_stat_1926402032_18_5_2]
    ON [dbo].[InputData]([Group], [CountryICAO], [IdentifyDocumentId]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_InputData_47_2002926307__K20_K2_K1_K19_3_4_5_6_7_8_9_10_11_12_13_14_15_16_17_18]
    ON [dbo].[InputData]([Group] ASC, [IdentifyDocumentId] ASC, [Id] ASC, [ShortFileIdentifier] ASC)
    INCLUDE([DocumentCategoryName], [CountryName], [CountryICAO], [Year], [Month], [RegisterCode], [RegisterNumber], [IssueDate], [ValidToDate], [InscribeId], [DataFormatId], [DataTypeId], [Order], [Dense], [XmlSize], [Size]);


GO
CREATE STATISTICS [_dta_stat_2002926307_20_19]
    ON [dbo].[InputData]([Group], [ShortFileIdentifier]);


GO
CREATE STATISTICS [_dta_stat_2002926307_2_20_19]
    ON [dbo].[InputData]([IdentifyDocumentId], [Group], [ShortFileIdentifier]);


GO
CREATE STATISTICS [_dta_stat_2002926307_2_1_20_19]
    ON [dbo].[InputData]([IdentifyDocumentId], [Id], [Group], [ShortFileIdentifier]);


GO
CREATE STATISTICS [_dta_stat_2002926307_1_19_2]
    ON [dbo].[InputData]([Id], [ShortFileIdentifier], [IdentifyDocumentId]);

