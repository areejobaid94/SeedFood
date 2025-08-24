CREATE TABLE [dbo].[Caption] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        BIGINT         NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   BIGINT         NULL,
    [IsDeleted]            BIT            NOT NULL,
    [DeleterUserId]        BIGINT         NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [TenantId]             INT            NULL,
    [Text]                 NVARCHAR (MAX) NULL,
    [LanguageBotId]        INT            NOT NULL,
    [TextResourceId]       INT            NOT NULL,
    CONSTRAINT [PK_Caption] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Caption_LanguageBot_LanguageBotId] FOREIGN KEY ([LanguageBotId]) REFERENCES [dbo].[LanguageBot] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Caption_TextResource_TextResourceId] FOREIGN KEY ([TextResourceId]) REFERENCES [dbo].[TextResource] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Caption_TextResourceId]
    ON [dbo].[Caption]([TextResourceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Caption_LanguageBotId]
    ON [dbo].[Caption]([LanguageBotId] ASC);

