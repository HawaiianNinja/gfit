CREATE TABLE [dbo].[GFIT_GAUNTLETS] (
    [id]            INT            IDENTITY (1, 1) NOT NULL,
    [excercise]     NVARCHAR (256) NOT NULL,
    [reps]          INT            NOT NULL,
    [difficulty]    INT            NOT NULL,
    [date_created]  DATETIME2 (7)  NOT NULL,
    [date_assigned] DATE           NOT NULL,
    CONSTRAINT [PK_GFitGauntlets] PRIMARY KEY CLUSTERED ([id] ASC)
);

