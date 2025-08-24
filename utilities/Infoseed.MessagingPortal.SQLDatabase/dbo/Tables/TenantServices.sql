CREATE TABLE [dbo].[TenantServices] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)   NOT NULL,
    [CreatorUserId]        BIGINT          NULL,
    [LastModificationTime] DATETIME2 (7)   NULL,
    [LastModifierUserId]   BIGINT          NULL,
    [IsDeleted]            BIT             NOT NULL,
    [DeleterUserId]        BIGINT          NULL,
    [DeletionTime]         DATETIME2 (7)   NULL,
    [TenantId]             INT             NULL,
    [ServiceFees]          DECIMAL (18, 2) NOT NULL,
    [InfoSeedServiceId]    INT             NULL,
    CONSTRAINT [PK_TenantServices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TenantServices_InfoSeedServices_InfoSeedServiceId] FOREIGN KEY ([InfoSeedServiceId]) REFERENCES [dbo].[InfoSeedServices] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_TenantServices_TenantId]
    ON [dbo].[TenantServices]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantServices_InfoSeedServiceId]
    ON [dbo].[TenantServices]([InfoSeedServiceId] ASC);

