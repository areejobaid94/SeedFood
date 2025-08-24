CREATE TABLE [dbo].[AccountBillings] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [TenantId]             INT             NULL,
    [BillID]               NVARCHAR (256)  NOT NULL,
    [BillDateFrom]         DATETIME2 (7)   NOT NULL,
    [BillDateTo]           DATETIME2 (7)   NOT NULL,
    [OpenAmount]           DECIMAL (18, 2) NULL,
    [BillAmount]           DECIMAL (18, 2) NULL,
    [InfoSeedServiceId]    INT             NULL,
    [ServiceTypeId]        INT             NULL,
    [CurrencyId]           INT             NULL,
    [BillingId]            INT             NULL,
    CONSTRAINT [PK_AccountBillings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AccountBillings_Billings_BillingId] FOREIGN KEY ([BillingId]) REFERENCES [dbo].[Billings] ([Id]),
    CONSTRAINT [FK_AccountBillings_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currencies] ([Id]),
    CONSTRAINT [FK_AccountBillings_InfoSeedServices_InfoSeedServiceId] FOREIGN KEY ([InfoSeedServiceId]) REFERENCES [dbo].[InfoSeedServices] ([Id]),
    CONSTRAINT [FK_AccountBillings_ServiceTypes_ServiceTypeId] FOREIGN KEY ([ServiceTypeId]) REFERENCES [dbo].[ServiceTypes] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_AccountBillings_BillingId]
    ON [dbo].[AccountBillings]([BillingId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AccountBillings_TenantId]
    ON [dbo].[AccountBillings]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AccountBillings_ServiceTypeId]
    ON [dbo].[AccountBillings]([ServiceTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AccountBillings_InfoSeedServiceId]
    ON [dbo].[AccountBillings]([InfoSeedServiceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AccountBillings_CurrencyId]
    ON [dbo].[AccountBillings]([CurrencyId] ASC);

