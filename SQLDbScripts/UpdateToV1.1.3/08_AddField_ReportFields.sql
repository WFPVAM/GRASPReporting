/*
   venerdì 16 gennaio 201509:02:59
   User: 
   Server: .\sqlexpress
   Database: GRASPPCB
   Application: 
*/

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
ALTER TABLE dbo.ReportFields ADD
	ReportFieldOrder int NULL
GO
ALTER TABLE dbo.ReportFields ADD CONSTRAINT
	DF_ReportFields_ReportFieldOrder DEFAULT 0 FOR ReportFieldOrder
GO
ALTER TABLE dbo.ReportFields SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.ReportFields', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.ReportFields', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.ReportFields', 'Object', 'CONTROL') as Contr_Per 