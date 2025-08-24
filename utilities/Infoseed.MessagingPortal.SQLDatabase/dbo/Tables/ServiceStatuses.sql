CREATE TABLE [dbo].[ServiceStatuses] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [ServiceStatusName] NVARCHAR (1024) NOT NULL,
    [IsEnabled]         BIT             NOT NULL,
    [CreationDate]      DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_ServiceStatuses] PRIMARY KEY CLUSTERED ([Id] ASC)
);

