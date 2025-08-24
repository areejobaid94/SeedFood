CREATE TABLE [dbo].[TemplateMessages] (
    [Id]                       INT              IDENTITY (1, 1) NOT NULL,
    [TenantId]                 INT              NULL,
    [TemplateMessageName]      NVARCHAR (256)   NOT NULL,
    [TemplateMessagePurposeId] INT              NOT NULL,
    [MessageCreationDate]      DATETIME2 (7)    DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [MessageText]              NVARCHAR (1024)  DEFAULT (N'') NOT NULL,
    [AttachmentId]             UNIQUEIDENTIFIER DEFAULT ('00000000-0000-0000-0000-000000000000') NOT NULL,
    CONSTRAINT [PK_TemplateMessages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TemplateMessages_TemplateMessagePurposes_TemplateMessagePurposeId] FOREIGN KEY ([TemplateMessagePurposeId]) REFERENCES [dbo].[TemplateMessagePurposes] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TemplateMessages_TenantId]
    ON [dbo].[TemplateMessages]([TenantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TemplateMessages_TemplateMessagePurposeId]
    ON [dbo].[TemplateMessages]([TemplateMessagePurposeId] ASC);

