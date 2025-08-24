CREATE TABLE [dbo].[OrderStatuses] (
    [Id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [TenantId] INT            NULL,
    [Name]     NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_OrderStatuses] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_OrderStatuses_TenantId]
    ON [dbo].[OrderStatuses]([TenantId] ASC);

