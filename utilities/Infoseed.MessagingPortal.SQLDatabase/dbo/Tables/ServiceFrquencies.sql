CREATE TABLE [dbo].[ServiceFrquencies] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [ServiceFrequencyName] NVARCHAR (1024) NOT NULL,
    [IsEnabled]            BIT             NOT NULL,
    [CreationDate]         DATETIME2 (7)   NOT NULL,
    [TotalNumberOfDays]    INT             NULL,
    CONSTRAINT [PK_ServiceFrquencies] PRIMARY KEY CLUSTERED ([Id] ASC)
);

