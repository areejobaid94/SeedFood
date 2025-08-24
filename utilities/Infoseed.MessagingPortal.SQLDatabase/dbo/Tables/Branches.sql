CREATE TABLE [dbo].[Branches] (
    [Id]             BIGINT          IDENTITY (1, 1) NOT NULL,
    [TenantId]       INT             NULL,
    [Name]           NVARCHAR (MAX)  NOT NULL,
    [DeliveryCost]   DECIMAL (18, 2) NULL,
    [RestaurantName] NVARCHAR (MAX)  NULL,
    [BranchAreaId]   INT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Branches] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Branches_TenantId]
    ON [dbo].[Branches]([TenantId] ASC);

