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