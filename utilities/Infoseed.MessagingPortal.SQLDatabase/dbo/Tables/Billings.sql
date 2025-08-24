CREATE TABLE [dbo].[Billings] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [TenantId]             INT             NULL,
    [BillingID]            NVARCHAR (256)  NOT NULL,
    [BillingDate]          DATETIME2 (7)   NOT NULL,
    [TotalAmount]          DECIMAL (18, 2) NOT NULL,
    [BillPeriodTo]         DATETIME2 (7)   NOT NULL,
    [BillPeriodFrom]       DATETIME2 (7)   NOT NULL,
    [DueDate]              DATETIME2 (7)   NOT NULL,
    [CurrencyId]           INT             NOT NULL,
    [IsPayed]              BIT             DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [TenantServiceId]      INT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Billings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Billings_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currencies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Billings_TenantServices_TenantServiceId] FOREIGN KEY ([TenantServiceId]) REFERENCES [dbo].[TenantServices] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Billings_TenantServiceId]
    ON [dbo].[Billings]([TenantServiceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Billings_TenantId]
    ON [dbo].[Billings]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Billings_CurrencyId]
    ON [dbo].[Billings]([CurrencyId] ASC);

