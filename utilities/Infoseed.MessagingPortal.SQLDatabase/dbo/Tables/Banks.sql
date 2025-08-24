CREATE TABLE [dbo].[Banks] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [BankName] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_Banks] PRIMARY KEY CLUSTERED ([Id] ASC)
);

