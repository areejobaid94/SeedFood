CREATE TABLE [dbo].[TextResource] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        BIGINT         NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   BIGINT         NULL,
    [IsDeleted]            BIT            NOT NULL,
    [DeleterUserId]        BIGINT         NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [TenantId]             INT            NULL,
    [Key]                  NVARCHAR (MAX) NULL,
    [Category]             NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TextResource] PRIMARY KEY CLUSTERED ([Id] ASC)
);

