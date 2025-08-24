CREATE TABLE [dbo].[LocationDeliveryCost] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [TenantId]     INT             NOT NULL,
    [LocationId]   INT             NOT NULL,
    [DeliveryCost] DECIMAL (18, 3) CONSTRAINT [DF_LocationDeliveryCost_DeliveryCost] DEFAULT ((0)) NOT NULL,
    [BranchAreaId] INT             NULL,
    CONSTRAINT [PK_LocationDeliveryCost] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LocationDeliveryCost_Locations] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id])
);

