CREATE TABLE [dbo].[OrderDetails] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [TenantId]             INT             NULL,
    [Quantity]             INT             NULL,
    [UnitPrice]            DECIMAL (18, 2) NULL,
    [Total]                DECIMAL (18, 2) NULL,
    [Discount]             DECIMAL (18, 2) NULL,
    [TotalAfterDiscunt]    DECIMAL (18, 2) NULL,
    [Tax]                  DECIMAL (18, 2) NULL,
    [TotalAfterTax]        DECIMAL (18, 2) NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [OrderId]              BIGINT          NULL,
    [ItemId]               BIGINT          NULL,
    CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderDetails_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]),
    CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_OrderDetails_TenantId]
    ON [dbo].[OrderDetails]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OrderDetails_OrderId]
    ON [dbo].[OrderDetails]([OrderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OrderDetails_ItemId]
    ON [dbo].[OrderDetails]([ItemId] ASC);

