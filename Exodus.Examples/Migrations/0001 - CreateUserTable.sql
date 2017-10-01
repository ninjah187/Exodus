CREATE TABLE [dbo].[Users] (
    [Id]       INT IDENTITY (1, 1) NOT NULL,
    [Email]    NVARCHAR (255) NOT NULL,
    [Password] NVARCHAR (255) NOT NULL,
    [Salt]     NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);