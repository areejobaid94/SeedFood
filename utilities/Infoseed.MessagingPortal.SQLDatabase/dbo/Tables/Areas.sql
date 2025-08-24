CREATE TABLE [dbo].[Areas] (
    [Id]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [TenantId]       INT            NULL,
    [AreaName]       NVARCHAR (450) NOT NULL,
    [AreaCoordinate] NVARCHAR (450) NULL,
    [BranchID]       NVARCHAR (MAX) NULL,
    [UserId]         INT            NULL,
    CONSTRAINT [PK_Areas] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_Areas_TenantId]
    ON [dbo].[Areas]([TenantId] ASC);

