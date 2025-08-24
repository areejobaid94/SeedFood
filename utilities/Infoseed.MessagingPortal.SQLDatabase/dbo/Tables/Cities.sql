CREATE TABLE [dbo].[Cities] (
    [Id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [TenantId] INT            NULL,
    [Name]     NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_Cities_TenantId]
    ON [dbo].[Cities]([TenantId] ASC);

