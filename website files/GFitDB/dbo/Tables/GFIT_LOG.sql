CREATE TABLE [dbo].[GFIT_LOG] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [log_text]   NVARCHAR (MAX) NOT NULL,
    [date_added] DATETIME2 (7)  CONSTRAINT [DF_GFIT_LOG_date_added] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_GFIT_LOG] PRIMARY KEY CLUSTERED ([id] ASC)
);

