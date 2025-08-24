CREATE TABLE [dbo].[ItemAdditions] (
    [Id]       BIGINT          IDENTITY (1, 1) NOT NULL,
    [TenantId] INT             NULL,
    [Name]     NVARCHAR (MAX)  NOT NULL,
    [Price]    DECIMAL (18, 2) NULL,
    [ItemId]   BIGINT          NULL,
    [SKU]      NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ItemAdditions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ItemAdditions_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ItemAdditions_ItemId]
    ON [dbo].[ItemAdditions]([ItemId] ASC);

