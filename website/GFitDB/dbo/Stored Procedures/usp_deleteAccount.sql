-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.usp_deleteAccount
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