CREATE TABLE [dbo].[TenantInformations] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7) NOT NULL,
    [CreatorUserId]        BIGINT        NULL,
    [LastModificationTime] DATETIME2 (7) NULL,
    [LastModifierUserId]   BIGINT        NULL,
    [IsDeleted]            BIT           NOT NULL,
    [DeleterUserId]        BIGINT        NULL,
    [DeletionTime]         DATETIME2 (7) NULL,
    [TenantId]             INT           NULL,
    [StartDate]            DATETIME2 (7) NOT NULL,
    [EndDate]              DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_TenantInformations] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TenantInformations_TenantId]
    ON [dbo].[TenantInformations]([TenantId] ASC);

