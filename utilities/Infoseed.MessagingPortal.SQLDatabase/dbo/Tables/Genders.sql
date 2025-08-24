CREATE TABLE [dbo].[Genders] (
    [Id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [TenantId] INT            NULL,
    [Name]     NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_Genders] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_Genders_TenantId]
    ON [dbo].[Genders]([TenantId] ASC);

