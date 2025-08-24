CREATE TABLE [dbo].[LocationLevels] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [LevelName] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_LocationLevels] PRIMARY KEY CLUSTERED ([Id] ASC)
);

