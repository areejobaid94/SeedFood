CREATE TABLE [dbo].[PaymentMethods] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [PaymnetMethod] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_PaymentMethods] PRIMARY KEY CLUSTERED ([Id] ASC)
);

