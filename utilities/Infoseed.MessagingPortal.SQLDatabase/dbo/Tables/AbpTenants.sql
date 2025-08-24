CREATE TABLE [dbo].[AbpTenants] (
    [Id]                       INT              IDENTITY (1, 1) NOT NULL,
    [ConnectionString]         NVARCHAR (1024)  NULL,
    [CreationTime]             DATETIME2 (7)    NOT NULL,
    [CreatorUserId]            BIGINT           NULL,
    [CustomCssId]              UNIQUEIDENTIFIER NULL,
    [DeleterUserId]            BIGINT           NULL,
    [DeletionTime]             DATETIME2 (7)    NULL,
    [EditionId]                INT              NULL,
    [IsActive]                 BIT              NOT NULL,
    [IsDeleted]                BIT              NOT NULL,
    [LastModificationTime]     DATETIME2 (7)    NULL,
    [LastModifierUserId]       BIGINT           NULL,
    [LogoFileType]             NVARCHAR (64)    NULL,
    [LogoId]                   UNIQUEIDENTIFIER NULL,
    [Name]                     NVARCHAR (128)   NOT NULL,
    [TenancyName]              NVARCHAR (64)    NOT NULL,
    [IsInTrialPeriod]          BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [SubscriptionEndDateUtc]   DATETIME2 (7)    NULL,
    [SubscriptionPaymentType]  INT              DEFAULT ((0)) NOT NULL,
    [CommercialName]           NVARCHAR (MAX)   NULL,
    [CustomerID]               INT              DEFAULT ((0)) NOT NULL,
    [CustomerLegalName]        NVARCHAR (MAX)   NULL,
    [Address]                  NVARCHAR (MAX)   NULL,
    [ContactPersonEmail]       NVARCHAR (MAX)   NULL,
    [ContactPersonName]        NVARCHAR (MAX)   NULL,
    [ContactPersonPhoneNumber] NVARCHAR (MAX)   NULL,
    [CustomerStatusID]         INT              DEFAULT ((0)) NOT NULL,
    [Email]                    NVARCHAR (MAX)   NULL,
    [LegalID]                  NVARCHAR (MAX)   NULL,
    [PhoneNumber]              NVARCHAR (MAX)   NULL,
    [SmoochAPIKeyID]           NVARCHAR (MAX)   NULL,
    [SmoochAppID]              NVARCHAR (MAX)   NULL,
    [SmoochSecretKey]          NVARCHAR (MAX)   NULL,
    [DirectLineSecret]         NVARCHAR (MAX)   NULL,
    [IsBotActive]              BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [botId]                    NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_AbpTenants] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AbpTenants_AbpEditions_EditionId] FOREIGN KEY ([EditionId]) REFERENCES [dbo].[AbpEditions] ([Id]),
    CONSTRAINT [FK_AbpTenants_AbpUsers_CreatorUserId] FOREIGN KEY ([CreatorUserId]) REFERENCES [dbo].[AbpUsers] ([Id]),
    CONSTRAINT [FK_AbpTenants_AbpUsers_DeleterUserId] FOREIGN KEY ([DeleterUserId]) REFERENCES [dbo].[AbpUsers] ([Id]),
    CONSTRAINT [FK_AbpTenants_AbpUsers_LastModifierUserId] FOREIGN KEY ([LastModifierUserId]) REFERENCES [dbo].[AbpUsers] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_AbpTenants_CreatorUserId]
    ON [dbo].[AbpTenants]([CreatorUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AbpTenants_DeleterUserId]
    ON [dbo].[AbpTenants]([DeleterUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AbpTenants_EditionId]
    ON [dbo].[AbpTenants]([EditionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AbpTenants_LastModifierUserId]
    ON [dbo].[AbpTenants]([LastModifierUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AbpTenants_TenancyName]
    ON [dbo].[AbpTenants]([TenancyName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AbpTenants_CreationTime]
    ON [dbo].[AbpTenants]([CreationTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AbpTenants_SubscriptionEndDateUtc]
    ON [dbo].[AbpTenants]([SubscriptionEndDateUtc] ASC);

