CREATE TABLE [dbo].[ReceiptDetails] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [BillingNumber]        NVARCHAR (256)  NULL,
    [BillDateFrom]         DATETIME2 (7)   NOT NULL,
    [BillDateTo]           DATETIME2 (7)   NOT NULL,
    [ServiceName]          NVARCHAR (1024) NOT NULL,
    [BillAmount]           DECIMAL (18, 2) NULL,
    [OpenAmount]           DECIMAL (18, 2) NULL,
    [CurrencyName]         NVARCHAR (256)  NOT NULL,
    [AccountBillingId]     INT             NULL,
    [ReceiptId]            INT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ReceiptDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReceiptDetails_AccountBillings_AccountBillingId] FOREIGN KEY ([AccountBillingId]) REFERENCES [dbo].[AccountBillings] ([Id]),
    CONSTRAINT [FK_ReceiptDetails_Receipts_ReceiptId] FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[Receipts] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ReceiptDetails_ReceiptId]
    ON [dbo].[ReceiptDetails]([ReceiptId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ReceiptDetails_AccountBillingId]
    ON [dbo].[ReceiptDetails]([AccountBillingId] ASC);

