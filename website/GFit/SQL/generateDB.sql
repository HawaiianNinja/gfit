USE [master]
GO
/****** Object:  Database [gFit]    Script Date: 8/18/2013 7:38:43 PM ******/
CREATE DATABASE [gFit]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'gFit_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\gFit.mdf' , SIZE = 3136KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'gFit_Log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\gFit.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [gFit] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [gFit].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [gFit] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [gFit] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [gFit] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [gFit] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [gFit] SET ARITHABORT OFF 
GO
ALTER DATABASE [gFit] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [gFit] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [gFit] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [gFit] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [gFit] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [gFit] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [gFit] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [gFit] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [gFit] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [gFit] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [gFit] SET  DISABLE_BROKER 
GO
ALTER DATABASE [gFit] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [gFit] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [gFit] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [gFit] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [gFit] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [gFit] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [gFit] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [gFit] SET RECOVERY FULL 
GO
ALTER DATABASE [gFit] SET  MULTI_USER 
GO
ALTER DATABASE [gFit] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [gFit] SET DB_CHAINING OFF 
GO
ALTER DATABASE [gFit] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [gFit] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [gFit]
GO
/****** Object:  StoredProcedure [dbo].[usp_addAccount]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_addAccount]
	
	--login method
	@oAuth bit = 1,

	--local stuff
	@username nvarchar(32) = null,
	@password nvarchar(256) = null,
	@firstname nvarchar(64) = null,
	@lastname nvarchar(64) = null,
	@dob date = null,
	@gender nvarchar(16) = null,

	--oAUTH stuff
	@provider_name nvarchar(64) = 'FACEBOOK',
	@authToken nvarchar(MAX) = null,
	@uid nvarchar(MAX) = null
	

AS
BEGIN


	IF (@oAuth = 1)
	BEGIN
	
		--check that we support the provider
		IF ((SELECT COUNT(*) FROM OAUTH_PROVIDERS P WHERE P.provider_name = @provider_name) = 0)
		BEGIN
			PRINT 'PROVIDER ' + @provider_name + ' IS NOT IN THE OAUTH_PROVIERS TABLE'
			RETURN -1;
		END

		--check that the username isn't null
		IF (@username IS NULL)
		BEGIN
			PRINT '@username CANNOT BE NULL';
		END

		--check that the username isn't currently used (if it is start adding nubmers)
		DECLARE @i int = 1;
		WHILE (SELECT COUNT(*) FROM ACCOUNTS A WHERE A.username = @username) <> 0
		BEGIN
			SET @username = @username + CONVERT(nvarchar(2),@i)
			SET @i = @i + 1
		END


		--grab the ID of the provider
		DECLARE @provider_id int
		SELECT @provider_id = id FROM OAUTH_PROVIDERS P WHERE P.provider_name=@provider_name


		--checks are done do insert
		BEGIN TRANSACTION

			INSERT INTO ACCOUNTS(oAuth, username, password, firstname, lastname, dob, gender)
			VALUES
			(@oAuth,@username, @password, @firstname, @lastname, @dob, @gender)


			INSERT INTO ACCOUNT_HAS_OAUTH (account_id, provider_id, uid, authToken)
			VALUES
			((SELECT id from ACCOUNTS A WHERE username = @username),
			 @provider_id, @uid, @authToken)
			
		COMMIT TRANSACTION 

		RETURN @@ERROR	
		
	END
	ELSE
	BEGIN
		PRINT 'CURRENTLY NON-OAUTH AUTHENTICATION IS NOT IMPLEMENTED'
		RETURN -1;
	END


END
GO
/****** Object:  StoredProcedure [dbo].[usp_addToLog]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_addToLog] 
	@log_text nvarchar(max) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO GFIT_LOG (log_text) values (@log_text)
END

GO
/****** Object:  StoredProcedure [dbo].[usp_deleteAccount]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_deleteAccount]
	@id int = null
AS
BEGIN

	if(@id IS NULL)
	BEGIN
		PRINT '@id CANNOT BE NULL'
		RETURN -1
	END

	ELSE
	BEGIN
	
		BEGIN TRANSACTION

		--delete records linking the account to a provider
		DELETE FROM ACCOUNT_HAS_OAUTH
		WHERE account_id = @id

		DELETE FROM ACCOUNTS
		WHERE id = @id

		COMMIT TRANSACTION
		
	
	END	



END
GO
/****** Object:  StoredProcedure [dbo].[usp_getAccountById]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_getAccountById]
	@id int = null
AS

	IF @id IS NULL
	BEGIN
		PRINT '@FacebookUid CANNOT BE NULL'
		RETURN -1
	END
	ELSE
	BEGIN


	SELECT 
		V.account_id,
		V.username,
		V.password,
		V.firstName,
		V.lastName,
		V.dob,
		V.gender,
		V.provider_name,
		V.authToken,
		V.uid
	FROM V_USERS_WITH_PROVIDERS V
	WHERE V.account_id = @id

	RETURN @@ERROR

	END
GO
/****** Object:  StoredProcedure [dbo].[usp_getAccountByOAuthUid]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_getAccountByOAuthUid]
	@Uid nvarchar(MAX) = null
AS

	IF @Uid IS NULL
	BEGIN
		PRINT '@FacebookUid CANNOT BE NULL'
		RETURN -1
	END
	ELSE
	BEGIN


	SELECT 
		V.account_id,
		V.username,
		V.password,
		V.firstName,
		V.lastName,
		V.dob,
		V.gender,
		V.provider_name,
		V.authToken,
		V.uid
	FROM V_USERS_WITH_PROVIDERS V
	WHERE V.uid = @Uid

	RETURN @@ERROR

	END
GO
/****** Object:  StoredProcedure [dbo].[usp_getGauntletByDate]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_getGauntletByDate]
	@date date = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF(@date IS NULL)
	BEGIN
		SET @date = GETDATE()
	END







   SELECT 
		G.id,
		G.excercise,
		G.reps,
		G.difficulty,
		G.date_created,
		G.date_assigned

   FROM GFIT_GAUNTLETS G
   --NOTE: Add this
   --WHERE date_assigned = @date



END
GO
/****** Object:  StoredProcedure [dbo].[usp_getGauntletById]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_getGauntletById]
	@gauntlet_id int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
   SELECT 
		G.id,
		G.excercise,
		G.reps,
		G.difficulty,
		G.date_created,
		G.date_assigned

   FROM GFIT_GAUNTLETS G
   WHERE id = @gauntlet_id



END
GO
/****** Object:  StoredProcedure [dbo].[usp_getGauntletSetById]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getGauntletSetById]
	@set_id int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	IF (@set_id IS NULL)
	BEGIN
		PRINT '@set_id CANNOT BE NULL'
		RETURN -1
	END
	

	SELECT 
		G.set_id,
		G.account_id,
		G.gauntlet_id,
		G.num_reps,
		G.start_time,
		G.end_time,
		G.completed

		FROM GAUNTLET_SETS G
		WHERE G.set_id = @set_id






END

GO
/****** Object:  StoredProcedure [dbo].[usp_newSet]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_newSet]
	@account_id int = null,
	@gauntlet_id int = null


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--verify @account_id
	IF(@account_id is null)
	BEGIN
		PRINT '@account_id CANNOT BE NULL'
		RETURN -1
	END

	--verify @gauntlet_id
	IF (@gauntlet_id is null)
	BEGIN
		PRINT '@gauntlet_id CANNOT BE NULL'
		RETURN -1
	END

	--verify account exists
	IF (SELECT COUNT(*) FROM ACCOUNTS WHERE id = @account_id) = 0
	BEGIN
		PRINT 'NO ID ' + Convert(nvarchar(10), @account_id) + ' EXISTS IN ACCOUNTS TABLE'
		RETURN -1
	END

	--verify gauntlet exists
	IF (SELECT COUNT(*) FROM GFIT_GAUNTLETS WHERE id = @gauntlet_id) = 0
	BEGIN
		PRINT 'NO ID ' + CONVERT(NVARCHAR(10), @gauntlet_id) + ' EXISTS IN GFIT_GAUNTLETS TABLE'
		RETURN -1
	END


	--verify that gantlet hasn't been completed
	DECLARE @repsDone int;
	SELECT @repsDone = SUM(num_reps) FROM GAUNTLET_SETS
		WHERE gauntlet_id = @gauntlet_id
		AND   account_id = @account_id
    

	IF(@repsDone >= (SELECT reps FROM GFIT_GAUNTLETS WHERE id = @gauntlet_id))
	BEGIN
		PRINT 'ACCOUNT ' + @account_id + ' HAS ALREADY COMPLETED GANTLET ' + @gauntlet_id
		RETURN -2
	END



	--check if they already have a new set created
	--perhaps, the refreshed a page or left the browser
	--if they do, return it

	IF((SELECT COUNT(*) FROM GAUNTLET_SETS GS
		WHERE	GS.account_id = @account_id
		AND		GS.gauntlet_id = @gauntlet_id
		AND		GS.completed = 0) <> 0) 
	BEGIN

		DECLARE @last_set_id int = null
		
		SELECT @last_set_id = set_id
			FROM GAUNTLET_SETS GS
			WHERE	GS.account_id = @account_id
			AND		GS.gauntlet_id = @gauntlet_id
			AND		GS.completed = 0

		EXEC usp_getGauntletSetById @set_id = @last_set_id

		RETURN @@ERROR
	END










	--make a new set and return it

	DECLARE @set_id int;
	SET @set_id = 0;

	IF(@repsDone > 0)
	BEGIN
		SELECT @set_id = MAX(set_id) FROM GAUNTLET_SETS 
			WHERE account_id = @account_id
			AND   gauntlet_id = @gauntlet_id
	END

	BEGIN TRANSACTION

		INSERT INTO GAUNTLET_SETS
		(set_id, account_id, gauntlet_id)
		VALUES
		(@set_id, @account_id, @gauntlet_id)


		SELECT 
			G.set_id,
			G.account_id,
			G.gauntlet_id,
			G.num_reps,
			G.start_time,
			G.end_time,
			G.completed

			FROM GAUNTLET_SETS G
			WHERE	G.set_id = (SELECT MAX(set_id) FROM GAUNTLET_SETS G2
								WHERE G2.account_id = @account_id
								AND G2.gauntlet_id = @gauntlet_id)
			AND		G.account_id = @account_id
			AND		G.gauntlet_id = @gauntlet_id


	COMMIT TRANSACTION


END
GO
/****** Object:  Table [dbo].[ACCOUNT_HAS_OAUTH]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ACCOUNT_HAS_OAUTH](
	[account_id] [int] NOT NULL,
	[provider_id] [int] NOT NULL,
	[authToken] [nvarchar](max) NOT NULL,
	[uid] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ACCOUNT_HAS_OAUTH] PRIMARY KEY CLUSTERED 
(
	[account_id] ASC,
	[provider_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ACCOUNTS]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ACCOUNTS](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[oAuth] [bit] NOT NULL,
	[username] [nvarchar](32) NOT NULL,
	[password] [nvarchar](256) NULL,
	[firstName] [nvarchar](64) NULL,
	[lastName] [nvarchar](64) NULL,
	[dob] [date] NULL,
	[gender] [nvarchar](16) NULL,
	[dateCreated] [datetime2](7) NOT NULL,
	[dateLastAccessed] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__ACCOUNTS__3213E83F6B869F75] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GAUNTLET_SETS]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GAUNTLET_SETS](
	[row_guid] [uniqueidentifier] NOT NULL,
	[set_id] [int] NOT NULL,
	[account_id] [int] NOT NULL,
	[gauntlet_id] [int] NOT NULL,
	[num_reps] [int] NOT NULL,
	[start_time] [datetime2](7) NOT NULL,
	[end_time] [datetime2](7) NULL,
	[completed] [bit] NOT NULL,
 CONSTRAINT [PK_GAUNTLET_SETS] PRIMARY KEY CLUSTERED 
(
	[set_id] ASC,
	[account_id] ASC,
	[gauntlet_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GFIT_GAUNTLETS]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GFIT_GAUNTLETS](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[excercise] [nvarchar](256) NOT NULL,
	[reps] [int] NOT NULL,
	[difficulty] [int] NOT NULL,
	[date_created] [datetime2](7) NOT NULL,
	[date_assigned] [date] NOT NULL,
 CONSTRAINT [PK_GFitGauntlets] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GFIT_LOG]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GFIT_LOG](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[log_text] [nvarchar](max) NOT NULL,
	[date_added] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_GFIT_LOG] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OAUTH_PROVIDERS]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OAUTH_PROVIDERS](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[provider_name] [nvarchar](64) NOT NULL,
 CONSTRAINT [PK_OAUTH_PROVIDERS] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[V_USERS_WITH_PROVIDERS]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[V_USERS_WITH_PROVIDERS]
AS

SELECT 
	A.id as [account_id],
	A.oAuth,
	A.username,
	A.password,
	A.firstName,
	A.lastName,
	A.dob,
	A.gender,
	P.id as [provider_id],
	P.provider_name as [provider_name] ,
	AHO.authToken,
	AHO.uid,
	A.dateCreated,
	A.dateLastAccessed



	FROM ACCOUNTS A
	LEFT OUTER JOIN ACCOUNT_HAS_OAUTH AHO on (A.id = AHO.account_id)
	Left Outer JOIN OAUTH_PROVIDERS P on (P.id = AHO.provider_id)
GO
/****** Object:  View [dbo].[V_SETS]    Script Date: 8/18/2013 7:38:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_SETS]

AS

SELECT 

	--set stuff
	S.set_id,
	S.num_reps,
	S.start_time,
	S.end_time,
	S.completed,


	--gauntlet stuff
	S.gauntlet_id,
	G.excercise,


	--account stuff
	V.account_id,
	V.username,
	V.uid

	    

FROM dbo.GAUNTLET_SETS S
	INNER JOIN dbo.GFIT_GAUNTLETS G ON (S.gauntlet_id = G.id)  
	INNER JOIN dbo.V_USERS_WITH_PROVIDERS V ON S.account_id = V.account_id
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_USERS_username]    Script Date: 8/18/2013 7:38:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_USERS_username] ON [dbo].[ACCOUNTS]
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ACCOUNTS] ADD  CONSTRAINT [DF__ACCOUNTS__dateCr__1273C1CD]  DEFAULT (getdate()) FOR [dateCreated]
GO
ALTER TABLE [dbo].[ACCOUNTS] ADD  CONSTRAINT [DF__ACCOUNTS__dateLa__1367E606]  DEFAULT (getdate()) FOR [dateLastAccessed]
GO
ALTER TABLE [dbo].[GAUNTLET_SETS] ADD  CONSTRAINT [DF_GAUNTLET_SETS_row_guid]  DEFAULT (newid()) FOR [row_guid]
GO
ALTER TABLE [dbo].[GAUNTLET_SETS] ADD  CONSTRAINT [DF_GAUNTLET_SETS_numberOfReps]  DEFAULT ((0)) FOR [num_reps]
GO
ALTER TABLE [dbo].[GAUNTLET_SETS] ADD  CONSTRAINT [DF_GAUNTLET_SETS_startTime]  DEFAULT (getdate()) FOR [start_time]
GO
ALTER TABLE [dbo].[GAUNTLET_SETS] ADD  CONSTRAINT [DF_GAUNTLET_SETS_completed]  DEFAULT ((0)) FOR [completed]
GO
ALTER TABLE [dbo].[GFIT_LOG] ADD  CONSTRAINT [DF_GFIT_LOG_date_added]  DEFAULT (getdate()) FOR [date_added]
GO
ALTER TABLE [dbo].[ACCOUNT_HAS_OAUTH]  WITH CHECK ADD  CONSTRAINT [FK_ACCOUNT_HAS_OAUTH_ACCOUNTS] FOREIGN KEY([account_id])
REFERENCES [dbo].[ACCOUNTS] ([id])
GO
ALTER TABLE [dbo].[ACCOUNT_HAS_OAUTH] CHECK CONSTRAINT [FK_ACCOUNT_HAS_OAUTH_ACCOUNTS]
GO
ALTER TABLE [dbo].[ACCOUNT_HAS_OAUTH]  WITH CHECK ADD  CONSTRAINT [FK_ACCOUNT_HAS_OAUTH_OAUTH_PROVIDERS] FOREIGN KEY([provider_id])
REFERENCES [dbo].[OAUTH_PROVIDERS] ([id])
GO
ALTER TABLE [dbo].[ACCOUNT_HAS_OAUTH] CHECK CONSTRAINT [FK_ACCOUNT_HAS_OAUTH_OAUTH_PROVIDERS]
GO
ALTER TABLE [dbo].[GAUNTLET_SETS]  WITH CHECK ADD  CONSTRAINT [FK_GAUNTLET_SETS_ACCOUNTS] FOREIGN KEY([account_id])
REFERENCES [dbo].[ACCOUNTS] ([id])
GO
ALTER TABLE [dbo].[GAUNTLET_SETS] CHECK CONSTRAINT [FK_GAUNTLET_SETS_ACCOUNTS]
GO
ALTER TABLE [dbo].[GAUNTLET_SETS]  WITH CHECK ADD  CONSTRAINT [FK_GAUNTLET_SETS_GFIT_GAUNTLETS] FOREIGN KEY([gauntlet_id])
REFERENCES [dbo].[GFIT_GAUNTLETS] ([id])
GO
ALTER TABLE [dbo].[GAUNTLET_SETS] CHECK CONSTRAINT [FK_GAUNTLET_SETS_GFIT_GAUNTLETS]
GO
USE [master]
GO
ALTER DATABASE [gFit] SET  READ_WRITE 
GO
