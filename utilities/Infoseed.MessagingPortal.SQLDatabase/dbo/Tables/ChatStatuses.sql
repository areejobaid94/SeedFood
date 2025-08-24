CREATE TABLE [dbo].[ChatStatuses] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [CreationTime]   DATETIME2 (7)   NOT NULL,
    [CreatorUserId]  BIGINT          NULL,
    [ChatStatusName] NVARCHAR (1024) NOT NULL,
    [IsEnabled]      BIT             NOT NULL,
    CONSTRAINT [PK_ChatStatuses] PRIMARY KEY CLUSTERED ([Id] ASC)
);

