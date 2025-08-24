CREATE TABLE [dbo].[ServiceTypes] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [ServicetypeName] NVARCHAR (1024) NOT NULL,
    [IsEnabled]       BIT             NOT NULL,
    [CreationDate]    DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_ServiceTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

