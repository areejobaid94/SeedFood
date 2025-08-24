CREATE TABLE [dbo].[ItemCategorys] (
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [TenantId]  INT            NULL,
    [Name]      NVARCHAR (450) NOT NULL,
    [IsDeleted] BIT            CONSTRAINT [DF__ItemCateg__IsDel__408F9238] DEFAULT (CONVERT([bit],(0))) NOT NULL,
    CONSTRAINT [PK_ItemCategorys] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ItemCategorys_TenantId]
    ON [dbo].[ItemCategorys]([TenantId] ASC);

