


CREATE PROCEDURE [dbo].[usp_getAccountByAccountID]
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