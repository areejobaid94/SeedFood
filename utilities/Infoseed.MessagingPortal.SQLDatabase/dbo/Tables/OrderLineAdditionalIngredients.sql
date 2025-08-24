CREATE TABLE [dbo].[OrderLineAdditionalIngredients] (
    [Id]        BIGINT          IDENTITY (1, 1) NOT NULL,
    [TenantId]  INT             NULL,
    [Remarks]   NVARCHAR (MAX)  NULL,
    [Total]     DECIMAL (18, 2) NULL,
    [Quantity]  INT             NULL,
    [UnitPrice] DECIMAL (18, 2) NULL,
    [OrderId]   BIGINT          NULL,
    CONSTRAINT [PK_OrderLineAdditionalIngredients] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderLineAdditionalIngredients_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_OrderLineAdditionalIngredients_TenantId]
    ON [dbo].[OrderLineAdditionalIngredients]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OrderLineAdditionalIngredients_OrderId]
    ON [dbo].[OrderLineAdditionalIngredients]([OrderId] ASC);

