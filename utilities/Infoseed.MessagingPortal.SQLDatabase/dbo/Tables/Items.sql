CREATE TABLE [dbo].[Items] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [TenantId]             INT             NULL,
    [ItemDescription]      NVARCHAR (MAX)  NULL,
    [Ingredients]          NVARCHAR (MAX)  NULL,
    [ItemName]             NVARCHAR (450)  NOT NULL,
    [IsInService]          BIT             NOT NULL,
    [CategoryNames]        NVARCHAR (MAX)  NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [ImageUri]             NVARCHAR (MAX)  NULL,
    [Price]                DECIMAL (18, 2) NULL,
    [Priority]             INT             CONSTRAINT [DF__Items__Priority__4183B671] DEFAULT ((0)) NOT NULL,
    [MenuId]               BIGINT          NULL,
    [ItemCategoryId]       BIGINT          NULL,
    [SKU]                  NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Items_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Items_TenantId]
    ON [dbo].[Items]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Items_MenuId]
    ON [dbo].[Items]([MenuId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Items_ItemCategoryId]
    ON [dbo].[Items]([ItemCategoryId] ASC);

