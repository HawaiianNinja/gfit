﻿/*
Deployment script for gFit

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "gFit"
:setvar DefaultFilePrefix "gFit"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET READ_COMMITTED_SNAPSHOT OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET PAGE_VERIFY NONE 
            WITH ROLLBACK IMMEDIATE;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Creating [dbo].[usp_storeCompletedSet]...';


GO
CREATE PROCEDURE [dbo].[usp_storeCompletedSet]
	@set_id int = null,
	@account_id int = null,
	@gauntlet_id int = null,	
	@num_reps int = null,
	@end_time datetime2(7) = null
AS
BEGIN	
	--check that @set_id isn't null
	IF(@set_id IS NULL)
	BEGIN
		PRINT '@set_id CANNOT BE NULL'
		RETURN -1
	END

	IF(@account_id IS NULL)
	BEGIN
		PRINT '@account_id CANNOT BE NULL'
		RETURN -1
	END

	IF(@gauntlet_id IS NULL)
	BEGIN
		PRINT '@gauntlet_id CANNOT BE NULL'
		RETURN -1
	END

	--check that numReps isn't 0
	if(@num_reps IS NULL) OR (@num_reps <= 0)
	BEGIN
		PRINT '@num_reps CANNOT BE NULL OR <= 0'
		RETURN -1
	END

	--check that such a record exists
	IF (SELECT COUNT(*) FROM GAUNTLET_SETS 
		WHERE	set_id = @set_id
		AND		account_id = @account_id
		AND		gauntlet_id = @gauntlet_id) = 0
	BEGIN
		PRINT 'RECORD DOES NOT EXIST';
		RETURN -2
	END


	--set @end_time = GETDATE() if null
	IF(@end_time IS NULL)
	BEGIN
		SET @end_time = GETDATE();
	END


	UPDATE GAUNTLET_SETS
	SET	num_reps = @num_reps,
		end_time = @end_time
	WHERE	set_id = @set_id
	AND		account_id = @account_id
	AND		gauntlet_id = @gauntlet_id

	RETURN @@ERROR




END
GO
PRINT N'Update complete.';


GO
