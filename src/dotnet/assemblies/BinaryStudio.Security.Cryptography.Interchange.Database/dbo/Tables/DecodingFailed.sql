CREATE TABLE [dbo].[DecodingFailed] (
    [Id]    INT            IDENTITY (1, 1) NOT NULL,
    [Key]   NVARCHAR(255)  NULL,
    [ShortFileIdentifier]  VARCHAR (50) NULL,
    [Group] NVARCHAR(50)   NOT NULL,
    CONSTRAINT [Failed_PK] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Failed_47_1990402260__K3_K2]
    ON [dbo].[DecodingFailed]([Group] ASC, [Key] ASC);

