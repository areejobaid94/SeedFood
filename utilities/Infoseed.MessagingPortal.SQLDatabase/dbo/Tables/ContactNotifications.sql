CREATE TABLE [dbo].[ContactNotifications] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [CreationTime]           DATETIME2 (7)  NOT NULL,
    [CreatorUserId]          BIGINT         NULL,
    [LastModificationTime]   DATETIME2 (7)  NULL,
    [LastModifierUserId]     BIGINT         NULL,
    [IsDeleted]              BIT            NOT NULL,
    [DeleterUserId]          BIGINT         NULL,
    [DeletionTime]           DATETIME2 (7)  NULL,
    [TenantId]               INT            NULL,
    [ContactId]              NVARCHAR (MAX) NOT NULL,
    [NotificationId]         NVARCHAR (MAX) NOT NULL,
    [NotificationCreateDate] DATETIME2 (7)  NOT NULL,
    [NotificationText]       NVARCHAR (MAX) NOT NULL,
    [UserId]                 INT            NOT NULL,
    CONSTRAINT [PK_ContactNotifications] PRIMARY KEY CLUSTERED ([Id] ASC)
);

