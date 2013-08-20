-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.usp_getGauntletSetsByAccount
	@account_id int = null,
	@gauntlet_id int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    

	--ensure @account_id isn't null
	IF(@account_id IS NULL)
	BEGIN
		PRINT '@account_id CANNOT BE NULL'
		RETURN -1
	END

	--ensure @gauntlet_id isn't null
	IF(@gauntlet_id IS NULL)
	BEGIN
		PRINT '@gauntlet_id CANNOT BE NULL'
		RETURN -1
	END

	

	SELECT 
			GS.set_id,
			GS.account_id,
			GS.gauntlet_id,
			GS.num_reps,
			GS.start_time,
			GS.end_time,
			GS.completed
	FROM GAUNTLET_SETS GS
	WHERE	GS.account_id = @account_id
	AND		GS.gauntlet_id = @gauntlet_id
	ORDER BY GS.set_id ASC

	

END