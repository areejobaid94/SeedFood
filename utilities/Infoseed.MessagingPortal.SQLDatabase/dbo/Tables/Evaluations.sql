CREATE TABLE [dbo].[Evaluations] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [TenantId]        INT            NULL,
    [OrderNumber]     BIGINT         NOT NULL,
    [ContactName]     NVARCHAR (MAX) NULL,
    [EvaluationsText] NVARCHAR (MAX) NOT NULL,
    [CreationTime]    DATETIME2 (7)  NOT NULL,
    [OrderId]         INT            NOT NULL,
    [PhoneNumber]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Evaluations] PRIMARY KEY CLUSTERED ([Id] ASC)
);

