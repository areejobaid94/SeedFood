CREATE TABLE [dbo].[DeliveryChanges] (
    [Id]                      BIGINT          IDENTITY (1, 1) NOT NULL,
    [CreatorUserId]           BIGINT          NULL,
    [LastModifierUserId]      BIGINT          NULL,
    [IsDeleted]               BIT             NOT NULL,
    [DeleterUserId]           BIGINT          NULL,
    [TenantId]                INT             NULL,
    [Charges]                 DECIMAL (18, 2) NULL,
    [DeliveryServiceProvider] NVARCHAR (450)  NULL,
    [CreationTime]            DATETIME2 (7)   NOT NULL,
    [DeletionTime]            DATETIME2 (7)   NULL,
    [LastModificationTime]    DATETIME2 (7)   NULL,
    [AreaId]                  BIGINT          NOT NULL,
    CONSTRAINT [PK_DeliveryChanges] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DeliveryChanges_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Areas] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_DeliveryChanges_TenantId]
    ON [dbo].[DeliveryChanges]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DeliveryChanges_AreaId]
    ON [dbo].[DeliveryChanges]([AreaId] ASC);

