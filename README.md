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
``