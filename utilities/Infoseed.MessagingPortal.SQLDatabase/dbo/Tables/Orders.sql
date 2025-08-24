CREATE TABLE [dbo].[Orders] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [TenantId]             INT             NULL,
    [OrderTime]            DATETIME2 (7)   NOT NULL,
    [OrderRemarks]         NVARCHAR (450)  NULL,
    [OrderNumber]          BIGINT          NOT NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [BranchId]             BIGINT          NULL,
    [ContactId]            INT             NULL,
    [AgentId]              BIGINT          DEFAULT (CONVERT([bigint],(0))) NOT NULL,
    [IsLockByAgent]        BIT             DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [LockByAgentName]      NVARCHAR (MAX)  NULL,
    [Total]                DECIMAL (18, 2) NULL,
    [OrderStatus]          INT             DEFAULT ((0)) NOT NULL,
    [OrderType]            INT             DEFAULT ((0)) NOT NULL,
    [Address]              NVARCHAR (MAX)  NULL,
    [AreaId]               BIGINT          NULL,
    [DeliveryCost]         DECIMAL (18, 2) NULL,
    [IsEvaluation]         BIT             DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [IsBranchArea]         BIT             DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [BranchAreaId]         INT             DEFAULT ((0)) NOT NULL,
    [BranchAreaName]       NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Areas] ([Id]),
    CONSTRAINT [FK_Orders_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]),
    CONSTRAINT [FK_Orders_Contacts_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contacts] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_Orders_TenantId]
    ON [dbo].[Orders]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Orders_ContactId]
    ON [dbo].[Orders]([ContactId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Orders_BranchId]
    ON [dbo].[Orders]([BranchId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Orders_AreaId]
    ON [dbo].[Orders]([AreaId] ASC);

