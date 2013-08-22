-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.usp_getGauntletSetById
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