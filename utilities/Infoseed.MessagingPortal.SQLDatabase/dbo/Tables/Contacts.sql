CREATE TABLE [dbo].[Contacts] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [TenantId]               INT            NULL,
    [AvatarUrl]              NVARCHAR (512) NULL,
    [DisplayName]            NVARCHAR (512) NULL,
    [PhoneNumber]            NVARCHAR (512) NULL,
    [SunshineAppID]          NVARCHAR (512) NULL,
    [IsLockedByAgent]        BIT            NOT NULL,
    [LockedByAgentName]      NVARCHAR (512) NULL,
    [IsOpen]                 BIT            NOT NULL,
    [Website]                NVARCHAR (512) NULL,
    [EmailAddress]           NVARCHAR (512) NULL,
    [Description]            NVARCHAR (MAX) NULL,
    [ChatStatuseId]          INT            NULL,
    [ContactStatuseId]       INT            NULL,
    [CreationTime]           DATETIME2 (7)  DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [CreatorUserId]          BIGINT         NULL,
    [DeleterUserId]          BIGINT         NULL,
    [DeletionTime]           DATETIME2 (7)  NULL,
    [IsDeleted]              BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [LastModificationTime]   DATETIME2 (7)  NULL,
    [LastModifierUserId]     BIGINT         NULL,
    [UserId]                 NVARCHAR (MAX) NULL,
    [IsConversationExpired]  BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [IsBlock]                BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [SunshineConversationId] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Contacts_ChatStatuses_ChatStatuseId] FOREIGN KEY ([ChatStatuseId]) REFERENCES [dbo].[ChatStatuses] ([Id]),
    CONSTRAINT [FK_Contacts_ContactStatuses_ContactStatuseId] FOREIGN KEY ([ContactStatuseId]) REFERENCES [dbo].[ContactStatuses] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Contacts_TenantId]
    ON [dbo].[Contacts]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Contacts_ContactStatuseId]
    ON [dbo].[Contacts]([ContactStatuseId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Contacts_ChatStatuseId]
    ON [dbo].[Contacts]([ChatStatuseId] ASC);

