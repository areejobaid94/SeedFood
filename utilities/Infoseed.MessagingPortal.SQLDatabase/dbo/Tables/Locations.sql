CREATE TABLE [dbo].[Locations] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [LocationName]   NVARCHAR (512) NOT NULL,
    [LevelId]        INT            NOT NULL,
    [ParentId]       INT            NULL,
    [GoogleURL]      NVARCHAR (MAX) NULL,
    [LocationNameEn] VARCHAR (512)  NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Locations_LocationLevels] FOREIGN KEY ([LevelId]) REFERENCES [dbo].[LocationLevels] ([Id]),
    CONSTRAINT [FK_Locations_Locations] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Locations] ([Id])
);

