CREATE TABLE [dbo].[ActiveContactPerMonth] (
    [Id]                  INT      IDENTITY (1, 1) NOT NULL,
    [TenantId]            INT      NOT NULL,
    [Year]                INT      NOT NULL,
    [Month]               INT      NOT NULL,
    [LastMessageDateTime] DATETIME CONSTRAINT [DF_ActiveContactPerMonth_ActiveContactCount] DEFAULT ((0)) NOT NULL,
    [ContactID]           INT      NULL,
    CONSTRAINT [PK_ActiveContactPerMonth] PRIMARY KEY CLUSTERED ([Id] ASC)
);

