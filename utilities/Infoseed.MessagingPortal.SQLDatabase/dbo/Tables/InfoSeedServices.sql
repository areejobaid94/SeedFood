CREATE TABLE [dbo].[InfoSeedServices] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [ServiceID]            NVARCHAR (1024) NOT NULL,
    [ServiceFees]          DECIMAL (18, 2) NOT NULL,
    [ServiceName]          NVARCHAR (1024) NOT NULL,
    [ServiceCreationDate]  DATETIME2 (7)   NOT NULL,
    [ServiceStoppingDate]  DATETIME2 (7)   NOT NULL,
    [Remarks]              NVARCHAR (MAX)  NULL,
    [ServiceTypeId]        INT             NOT NULL,
    [ServiceStatusId]      INT             NOT NULL,
    [ServiceFrquencyId]    INT             NOT NULL,
    [IsFeesPerTransaction] BIT             DEFAULT (CONVERT([bit],(0))) NOT NULL,
    CONSTRAINT [PK_InfoSeedServices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InfoSeedServices_ServiceFrquencies_ServiceFrquencyId] FOREIGN KEY ([ServiceFrquencyId]) REFERENCES [dbo].[ServiceFrquencies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_InfoSeedServices_ServiceStatuses_ServiceStatusId] FOREIGN KEY ([ServiceStatusId]) REFERENCES [dbo].[ServiceStatuses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_InfoSeedServices_ServiceTypes_ServiceTypeId] FOREIGN KEY ([ServiceTypeId]) REFERENCES [dbo].[ServiceTypes] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_InfoSeedServices_ServiceTypeId]
    ON [dbo].[InfoSeedServices]([ServiceTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_InfoSeedServices_ServiceStatusId]
    ON [dbo].[InfoSeedServices]([ServiceStatusId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_InfoSeedServices_ServiceFrquencyId]
    ON [dbo].[InfoSeedServices]([ServiceFrquencyId] ASC);

