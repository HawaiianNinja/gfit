

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