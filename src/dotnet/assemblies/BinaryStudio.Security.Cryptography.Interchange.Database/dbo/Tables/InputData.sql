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

