
CREATE PROCEDURE dbo.usp_getGauntletById
	@gauntlet_id int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
   SELECT 
		G.id,
		G.excercise,
		G.reps,
		G.difficulty,
		G.date_created,
		G.date_assigned

   FROM GFIT_GAUNTLETS G
   WHERE id = @gauntlet_id



END