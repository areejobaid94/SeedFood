CREATE TABLE [dbo].[AbpDynamicPropertyValues] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Value]             NVARCHAR (MAX) NOT NULL,
    [TenantId]          INT            NULL,
    [DynamicPropertyId] INT            NOT NULL,
    CONSTRAINT [PK_AbpDynamicPropertyValues] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AbpDynamicPropertyValues_AbpDynamicProperties_DynamicPropertyId] FOREIGN KEY ([DynamicPropertyId]) REFERENCES [dbo].[AbpDynamicProperties] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AbpDynamicPropertyValues_DynamicPropertyId]
    ON [dbo].[AbpDynamicPropertyValues]([DynamicPropertyId] ASC);

