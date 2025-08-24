CREATE TABLE [dbo].[OrderReceipts] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [TenantId]             INT             NULL,
    [OrderTime]            DATETIME2 (7)   NULL,
    [OrderAmount]          INT             NULL,
    [OrderDiscount]        DECIMAL (18, 2) NULL,
    [TotalAfterDiscunt]    DECIMAL (18, 2) NULL,
    [IsCashReceived]       BIT             NOT NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [OrderId]              BIGINT          NULL,
    CONSTRAINT [PK_OrderReceipts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderReceipts_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_OrderReceipts_TenantId]
    ON [dbo].[OrderReceipts]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OrderReceipts_OrderId]
    ON [dbo].[OrderReceipts]([OrderId] ASC);

