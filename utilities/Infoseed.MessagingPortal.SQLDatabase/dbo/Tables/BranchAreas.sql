CREATE TABLE [dbo].[BranchAreas] (
    [Id]                   BIGINT        IDENTITY (1, 1) NOT NULL,
    [CreatorUserId]        BIGINT        NULL,
    [LastModificationTime] DATETIME2 (7) NULL,
    [LastModifierUserId]   BIGINT        NULL,
    [IsDeleted]            BIT           NOT NULL,
    [DeleterUserId]        BIGINT        NULL,
    [DeletionTime]         DATETIME2 (7) NULL,
    [TenantId]             INT           NULL,
    [CreationTime]         DATETIME2 (7) NOT NULL,
    [AreaId]               BIGINT        NOT NULL,
    [BranchId]             BIGINT        DEFAULT (CONVERT([bigint],(0))) NOT NULL,
    CONSTRAINT [PK_BranchAreas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BranchAreas_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Areas] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BranchAreas_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_BranchAreas_TenantId]
    ON [dbo].[BranchAreas]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BranchAreas_BranchId]
    ON [dbo].[BranchAreas]([BranchId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BranchAreas_AreaId]
    ON [dbo].[BranchAreas]([AreaId] ASC);

