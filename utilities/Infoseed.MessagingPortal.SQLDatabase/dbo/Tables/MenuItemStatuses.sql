CREATE TABLE [dbo].[MenuItemStatuses] (
    [Id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_MenuItemStatuses] PRIMARY KEY CLUSTERED ([Id] ASC)
);



