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

