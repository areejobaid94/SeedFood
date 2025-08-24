CREATE TABLE [dbo].[LanguageBot] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        BIGINT         NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   BIGINT         NULL,
    [IsDeleted]            BIT            NOT NULL,
    [DeleterUserId]        BIGINT         NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [TenantId]             INT            NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    [ISO]                  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_LanguageBot] PRIMARY KEY CLUSTERED ([Id] ASC)
);

