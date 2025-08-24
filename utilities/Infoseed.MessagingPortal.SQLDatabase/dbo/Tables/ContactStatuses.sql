CREATE TABLE [dbo].[ContactStatuses] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [CreationTime]      DATETIME2 (7)   NOT NULL,
    [CreatorUserId]     BIGINT          NULL,
    [ContactStatusName] NVARCHAR (1024) NOT NULL,
    [IsEnabled]         BIT             NOT NULL,
    CONSTRAINT [PK_ContactStatuses] PRIMARY KEY CLUSTERED ([Id] ASC)
);

