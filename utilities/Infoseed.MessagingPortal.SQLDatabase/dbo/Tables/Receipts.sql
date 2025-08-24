CREATE TABLE [dbo].[Receipts] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [CreationTime]           DATETIME2 (7)  NOT NULL,
    [CreatorUserId]          BIGINT         NULL,
    [LastModificationTime]   DATETIME2 (7)  NULL,
    [LastModifierUserId]     BIGINT         NULL,
    [IsDeleted]              BIT            NOT NULL,
    [DeleterUserId]          BIGINT         NULL,
    [DeletionTime]           DATETIME2 (7)  NULL,
    [TenantId]               INT            NULL,
    [ReceiptNumber]          NVARCHAR (256) NOT NULL,
    [ReceiptDate]            DATETIME2 (7)  NOT NULL,
    [PaymentReferenceNumber] NVARCHAR (10)  NULL,
    [BankId]                 INT            NULL,
    [PaymentMethodId]        INT            NOT NULL,
    CONSTRAINT [PK_Receipts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Receipts_Banks_BankId] FOREIGN KEY ([BankId]) REFERENCES [dbo].[Banks] ([Id]),
    CONSTRAINT [FK_Receipts_PaymentMethods_PaymentMethodId] FOREIGN KEY ([PaymentMethodId]) REFERENCES [dbo].[PaymentMethods] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Receipts_TenantId]
    ON [dbo].[Receipts]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Receipts_PaymentMethodId]
    ON [dbo].[Receipts]([PaymentMethodId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Receipts_BankId]
    ON [dbo].[Receipts]([BankId] ASC);

