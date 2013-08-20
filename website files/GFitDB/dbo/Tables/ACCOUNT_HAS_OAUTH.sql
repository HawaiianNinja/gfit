CREATE TABLE [dbo].[ACCOUNT_HAS_OAUTH] (
    [account_id]  INT            NOT NULL,
    [provider_id] INT            NOT NULL,
    [authToken]   NVARCHAR (MAX) NOT NULL,
    [uid]         NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ACCOUNT_HAS_OAUTH] PRIMARY KEY CLUSTERED ([account_id] ASC, [provider_id] ASC),
    CONSTRAINT [FK_ACCOUNT_HAS_OAUTH_ACCOUNTS] FOREIGN KEY ([account_id]) REFERENCES [dbo].[ACCOUNTS] ([id]),
    CONSTRAINT [FK_ACCOUNT_HAS_OAUTH_OAUTH_PROVIDERS] FOREIGN KEY ([provider_id]) REFERENCES [dbo].[OAUTH_PROVIDERS] ([id])
);

