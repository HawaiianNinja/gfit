USE [master]
GO
/****** Object:  Database [gFit]    Script Date: 8/14/2013 1:18:46 AM ******/
CREATE DATABASE [gFit]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'fFit_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\fFit.mdf' , SIZE = 3136KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'fFit_Log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\fFit.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
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
/****** Object:  StoredProcedure [dbo].[usp_addAccount]    Script Date: 8/14/2013 1:18:46 AM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_getAccountByOAuthUid]    Script Date: 8/14/2013 1:18:46 AM ******/
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

	RETURN @@ERROR

	END


GO
/****** Object:  Table [dbo].[ACCOUNT_HAS_OAUTH]    Script Date: 8/14/2013 1:18:46 AM ******/
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
/****** Object:  Table [dbo].[ACCOUNTS]    Script Date: 8/14/2013 1:18:46 AM ******/
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
/****** Object:  Table [dbo].[OAUTH_PROVIDERS]    Script Date: 8/14/2013 1:18:46 AM ******/
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
/****** Object:  View [dbo].[V_USERS_WITH_PROVIDERS]    Script Date: 8/14/2013 1:18:46 AM ******/
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
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_USERS_username]    Script Date: 8/14/2013 1:18:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_USERS_username] ON [dbo].[ACCOUNTS]
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ACCOUNTS] ADD  CONSTRAINT [DF__ACCOUNTS__dateCr__1273C1CD]  DEFAULT (getdate()) FOR [dateCreated]
GO
ALTER TABLE [dbo].[ACCOUNTS] ADD  CONSTRAINT [DF__ACCOUNTS__dateLa__1367E606]  DEFAULT (getdate()) FOR [dateLastAccessed]
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
USE [master]
GO
ALTER DATABASE [gFit] SET  READ_WRITE 
GO
