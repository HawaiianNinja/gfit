

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