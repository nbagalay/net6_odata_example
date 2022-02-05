# .Net Core 6 With OData and Entity Framework

## What is this?
The following repository is a guided example of building an OData service using the C# .Net 6. We'll cover the very basics to getting more involved in authentication and tracking. Throughout this guide, please feel free to post examples and shoutouts. Hope this guide helps others build a more _RESTFUL_ application.

## Developer Notes
This model supports SQLite for Development. You don't need to have anything else installed. Follow the GUIDE below to scaffold the Entity Framework.

### EntityFramework for Project Tracker
Anytime a model is changed, a new migration is needed. This is anything deemed as, what we call "break fix". Removing a field, renaming are just a few examples when to run a new migration.

As a quick reference, here are the migration calls for EF. These commands can be ran in Visual Studio under the _Package Manager Console_:

**Note**: Migration tools will focus on the DEV environment. You must run the command in Package Manager Console below prior to executing _if_ you want to make updates to production (SQLServer Migrations). Use 'Development' if in Dev:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
```

* _Add-Migration { Enter a nice descriptive name here Ex: 'UpdateAuthors' } -c {Context model 'OdataNet6TutorialContext' } -o Migrations_ : This call will Create a new migration for the respective Context model. All items in brackets do not include the single '.
* _Remove-Migration -c {Context model 'OdataNet6TutorialContext'}_ : This will remove the last created migration. Note: All items in brackets do not include the single '.
* _Update-Database -Context {Context model 'OdataNet6TutorialContext'}_: This will update the respecitve DB to the latest model. 

## Tutorial - Using the Branch History
To make this easier for everyone, each branch history is adding features discussed in that tutorial. Below is a list of each Branch's History and what it achieved:

### Part 1 Branch - Fundamental CRUD Operations
This is the 'first' release of this tutorial. We cover:

* _Building Models_: We created 2 models Authors and Books and show some basic fields and navigation
* _Controllers_: The Controllers show how each of the different CRUD (Create, Read, Update and Delete) methods work. It's what drives how the OData fetchs and works!
* _Program.cs_: THIS IS NEW for those who have been in the WEB API space. It use to be Startup.cs. This is the core heart beat and has been greatly improved in .NET 6. This is where the foundation of the OData service is established and executed.

Since this is the FIRST run, you'll need to Run the 'Update' EF Code in the Visual Studio Console so that the *.db file will be created.

### Part 2 Branch - Adding JWT Authentication to your OData Service
*Coming NEXT!*