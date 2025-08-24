CREATE TABLE [dbo].[AppBinaryObjects] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Bytes]    VARBINARY (MAX)  NOT NULL,
    [TenantId] INT              NULL,
    [FileName] NVARCHAR (MAX)   NULL,
    [MimeType] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_AppBinaryObjects] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_AppBinaryObjects_TenantId]
    ON [dbo].[AppBinaryObjects]([TenantId] ASC);

