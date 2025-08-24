CREATE TABLE [dbo].[Customers] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreatorUserId]        BIGINT         NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   BIGINT         NULL,
    [DeleterUserId]        BIGINT         NULL,
    [TenantId]             INT            NULL,
    [CustomerName]         NVARCHAR (150) NOT NULL,
    [PhoneNumber]          NVARCHAR (100) NULL,
    [CustomerAddress]      NVARCHAR (MAX) NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [EmailAddress]         NVARCHAR (256) NOT NULL,
    [IsActive]             BIT            NOT NULL,
    [IsDeleted]            BIT            NOT NULL,
    [GenderId]             BIGINT         NULL,
    [CityId]               BIGINT         NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Customers_Cities_CityId] FOREIGN KEY ([CityId]) REFERENCES [dbo].[Cities] ([Id]),
    CONSTRAINT [FK_Customers_Genders_GenderId] FOREIGN KEY ([GenderId]) REFERENCES [dbo].[Genders] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Customers_TenantId]
    ON [dbo].[Customers]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Customers_GenderId]
    ON [dbo].[Customers]([GenderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Customers_CityId]
    ON [dbo].[Customers]([CityId] ASC);

