# IT Partners README.MD file

## Summary: 

This is the TQII (pronounced 'Tee-kee') Language test specialized for recording information. 

## Production location: 

None, currently. Will be an Azure App Service. 

## Development location: 

Currently, none. We do development on local machines, and will host temporary sites when end users need to see development work. 

## How to deploy to production/development: 

Will be CI/CD 

## How to set up locally: 

Run EF to set up a database, using user secrets to put in the connection string.

``
Update-Database -Context LanguageDbContext

Update-Database -Context RegistrationDbContext
``

This uses the default Identity framework, so add the tables for the identity framework.

``
Update-Database -Context ApplicationDbContext
``

Both systems use the DefaultConnection connection string. 


## Notes (error logging, external tools, links, etc.):

If you make changes to the database, make sure you add the changes to update the database.

``
Add-Migration -Name {name} -Context LanguageDbContext

Add-Migration -Name {name} -Context RegistrationDbContext
``

Need to add the stored procedure dbo.ResetReviewerStats - this is to reset the counts for completed ratings, all ratings, and unfinished ratings.

``
CREATE PROCEDURE [dbo].[ResetReviewerStats]
AS
BEGIN
    SET NOCOUNT ON

	UPDATE dbo.TestUsers SET NumberReviewers = 0, NumberReviewerScores = 0;
	UPDATE dbo.RaterNames SET NumberOfTests = 0;

    UPDATE tu 
    SET tu.NumberReviewers = rt.AllRatings
    FROM dbo.TestUsers tu
    INNER JOIN (SELECT TestUserId, COUNT(*) AS AllRatings 
    FROM dbo.RaterTests
	WHERE IsFinalScorer = 0
    GROUP BY TestUserId) rt
    ON tu.id = rt.TestUserId

    UPDATE tu 
    SET tu.NumberReviewerScores = rt.CompletedRatings 
    FROM dbo.TestUsers tu
    INNER JOIN (SELECT TestUserId, COUNT(*) AS CompletedRatings 
    FROM dbo.RaterTests
    WHERE DateFinished IS NOT NULL AND IsFinalScorer = 0
    GROUP BY TestUserId) rt
    ON tu.id = rt.TestUserId

    UPDATE rn
    SET rn.NumberOfTests = rt.UnfinishedRatings 
    FROM dbo.RaterNames rn 
    INNER JOIN (SELECT RaterNameId, COUNT(*) AS UnfinishedRatings 
    FROM dbo.RaterTests
    WHERE DateFinished IS NULL 
    GROUP BY RaterNameId) rt
    ON rn.id = rt.RaterNameId

END
GO
``