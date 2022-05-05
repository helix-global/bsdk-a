CREATE TABLE [dbo].[Country] (
    [Country]          VARCHAR(5) NOT NULL,
    [EnglishShortName] NVARCHAR (255) NOT NULL,
    [RussianShortName] NVARCHAR (255) NOT NULL,
    CONSTRAINT [Country_PK] PRIMARY KEY CLUSTERED ([Country] ASC),
);

