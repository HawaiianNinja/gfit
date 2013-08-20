CREATE TABLE [dbo].[ACCOUNTS] (
    [id]               INT            IDENTITY (1, 1) NOT NULL,
    [oAuth]            BIT            NOT NULL,
    [username]         NVARCHAR (32)  NOT NULL,
    [password]         NVARCHAR (256) NULL,
    [firstName]        NVARCHAR (64)  NULL,
    [lastName]         NVARCHAR (64)  NULL,
    [dob]              DATE           NULL,
    [gender]           NVARCHAR (16)  NULL,
    [dateCreated]      DATETIME2 (7)  CONSTRAINT [DF__ACCOUNTS__dateCr__1273C1CD] DEFAULT (getdate()) NOT NULL,
    [dateLastAccessed] DATETIME2 (7)  CONSTRAINT [DF__ACCOUNTS__dateLa__1367E606] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK__ACCOUNTS__3213E83F6B869F75] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_USERS_username]
    ON [dbo].[ACCOUNTS]([username] ASC);

