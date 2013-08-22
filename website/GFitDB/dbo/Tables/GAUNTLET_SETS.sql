CREATE TABLE [dbo].[GAUNTLET_SETS] (
    [row_guid]    UNIQUEIDENTIFIER CONSTRAINT [DF_GAUNTLET_SETS_row_guid] DEFAULT (newid()) NOT NULL,
    [set_id]      INT              NOT NULL,
    [account_id]  INT              NOT NULL,
    [gauntlet_id] INT              NOT NULL,
    [num_reps]    INT              CONSTRAINT [DF_GAUNTLET_SETS_numberOfReps] DEFAULT ((0)) NOT NULL,
    [start_time]  DATETIME2 (7)    CONSTRAINT [DF_GAUNTLET_SETS_startTime] DEFAULT (getdate()) NOT NULL,
    [end_time]    DATETIME2 (7)    NULL,
    [completed]   BIT              CONSTRAINT [DF_GAUNTLET_SETS_completed] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_GAUNTLET_SETS] PRIMARY KEY CLUSTERED ([set_id] ASC, [account_id] ASC, [gauntlet_id] ASC),
    CONSTRAINT [FK_GAUNTLET_SETS_ACCOUNTS] FOREIGN KEY ([account_id]) REFERENCES [dbo].[ACCOUNTS] ([id]),
    CONSTRAINT [FK_GAUNTLET_SETS_GFIT_GAUNTLETS] FOREIGN KEY ([gauntlet_id]) REFERENCES [dbo].[GFIT_GAUNTLETS] ([id])
);

