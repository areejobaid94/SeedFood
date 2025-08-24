CREATE TABLE [dbo].[Currencies] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [CreationTime]  DATETIME2 (7)  NOT NULL,
    [CreatorUserId] BIGINT         NULL,
    [CurrencyName]  NVARCHAR (256) NOT NULL,
    [ISOName]       NVARCHAR (3)   NOT NULL,
    CONSTRAINT [PK_Currencies] PRIMARY KEY CLUSTERED ([Id] ASC)
);

