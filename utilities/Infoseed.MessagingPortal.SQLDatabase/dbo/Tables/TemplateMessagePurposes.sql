CREATE TABLE [dbo].[TemplateMessagePurposes] (
    [Id]      INT             IDENTITY (1, 1) NOT NULL,
    [Purpose] NVARCHAR (1024) NOT NULL,
    CONSTRAINT [PK_TemplateMessagePurposes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

