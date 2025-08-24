CREATE TABLE [dbo].[ExtraOrderDetail] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [TenantId]      INT             NULL,
    [Name]          NVARCHAR (MAX)  NULL,
    [Quantity]      INT             NULL,
    [UnitPrice]     DECIMAL (18, 2) NULL,
    [Total]         DECIMAL (18, 2) NULL,
    [OrderDetailId] BIGINT          NULL,
    CONSTRAINT [PK_ExtraOrderDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExtraOrderDetail_OrderDetails_OrderDetailId] FOREIGN KEY ([OrderDetailId]) REFERENCES [dbo].[OrderDetails] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ExtraOrderDetail_OrderDetailId]
    ON [dbo].[ExtraOrderDetail]([OrderDetailId] ASC);

