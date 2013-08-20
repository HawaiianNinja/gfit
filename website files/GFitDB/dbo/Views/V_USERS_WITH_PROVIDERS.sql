
CREATE VIEW [dbo].[V_USERS_WITH_PROVIDERS]
AS

SELECT 
	A.id as [account_id],
	A.oAuth,
	A.username,
	A.password,
	A.firstName,
	A.lastName,
	A.dob,
	A.gender,
	P.id as [provider_id],
	P.provider_name as [provider_name] ,
	AHO.authToken,
	AHO.uid,
	A.dateCreated,
	A.dateLastAccessed



	FROM ACCOUNTS A
	LEFT OUTER JOIN ACCOUNT_HAS_OAUTH AHO on (A.id = AHO.account_id)
	Left Outer JOIN OAUTH_PROVIDERS P on (P.id = AHO.provider_id)