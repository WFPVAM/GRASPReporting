/*
   12 November 201421:02:47
   User: 
   Server: .\sqlexpress
   Database: GRASP
   Application: 
*/
USE [GRASP]
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.RolesToResponseStatus
	(
	RoleID int NOT NULL,
	ResponseStatusID int NOT NULL,
	RoleToRespStatusTypeID int NOT NULL,
	RoleToRespStatusID int NOT NULL IDENTITY (1, 1)
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.RolesToResponseStatus ADD CONSTRAINT
	PK_RolesToResponseStatus PRIMARY KEY CLUSTERED 
	(
	RoleToRespStatusID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.RolesToResponseStatus SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.RolesToResponseStatus', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.RolesToResponseStatus', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.RolesToResponseStatus', 'Object', 'CONTROL') as Contr_Per 