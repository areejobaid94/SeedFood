CREATE TABLE [dbo].[MenuDetails] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [TenantId]     INT             NULL,
    [Description]  NVARCHAR (MAX)  NULL,
    [IsStandAlone] BIT             NOT NULL,
    [Price]        DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_MenuDetails] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_MenuDetails_TenantId]
    ON [dbo].[MenuDetails]([TenantId] ASC);

