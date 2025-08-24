CREATE TABLE [dbo].[Menus] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [TenantId]          INT             NULL,
    [MenuName]          NVARCHAR (450)  NOT NULL,
    [MenuDescription]   NVARCHAR (MAX)  NULL,
    [EffectiveTimeFrom] DATETIME2 (7)   NULL,
    [EffectiveTimeTo]   DATETIME2 (7)   NULL,
    [Tax]               DECIMAL (18, 2) NULL,
    [ImageUri]          NVARCHAR (450)  NULL,
    [Priority]          INT             CONSTRAINT [DF__Menus__Priority__79FD19BE] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Menus] PRIMARY KEY CLUSTERED ([Id] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_Menus_TenantId]
    ON [dbo].[Menus]([TenantId] ASC);

