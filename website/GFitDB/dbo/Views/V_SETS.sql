CREATE VIEW V_SETS

AS

SELECT 

	--set stuff
	S.set_id,
	S.num_reps,
	S.start_time,
	S.end_time,
	S.completed,


	--gauntlet stuff
	S.gauntlet_id,
	G.excercise,


	--account stuff
	V.account_id,
	V.username,
	V.uid

	    

FROM dbo.GAUNTLET_SETS S
	INNER JOIN dbo.GFIT_GAUNTLETS G ON (S.gauntlet_id = G.id)  
	INNER JOIN dbo.V_USERS_WITH_PROVIDERS V ON S.account_id = V.account_id