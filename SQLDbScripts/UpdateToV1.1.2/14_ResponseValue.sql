/*
   07 November 201418:26:18
   User: Marco Giorgi @ Brains Engineering
   Server: .\sqlexpress
   Database: GRASP
   Application: GRASP Reporting
   
   This script extends the dimension of value field up to 4000 characters.
   The table will be dropped and recreated, migrating the existing data in the new table.
   --> If you already have data in your GRASP DB, 
       you must make a full backup of your database before running this script <--
*/
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
ALTER TABLE dbo.ResponseValue
	DROP CONSTRAINT DF_ResponseValue_RVCreateDate
GO
CREATE TABLE dbo.Tmp_ResponseValue
	(
	id numeric(19, 0) NOT NULL IDENTITY (1, 1),
	id_flsmsId nvarchar(255) NULL,
	pushed tinyint NOT NULL,
	value nvarchar(4000) NULL,
	RVRepeatCount int NULL,
	FormResponseID int NULL,
	RVCreateDate datetime NULL,
	formFieldId int NULL,
	positionIndex int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_ResponseValue SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_ResponseValue ADD CONSTRAINT
	DF_ResponseValue_RVCreateDate DEFAULT (getdate()) FOR RVCreateDate
GO
SET IDENTITY_INSERT dbo.Tmp_ResponseValue ON
GO
IF EXISTS(SELECT * FROM dbo.ResponseValue)
	 EXEC('INSERT INTO dbo.Tmp_ResponseValue (id, id_flsmsId, pushed, value, RVRepeatCount, FormResponseID, RVCreateDate, formFieldId, positionIndex)
		SELECT id, id_flsmsId, pushed, value, RVRepeatCount, FormResponseID, RVCreateDate, formFieldId, positionIndex FROM dbo.ResponseValue WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_ResponseValue OFF
GO
ALTER TABLE dbo.FormResponse_ResponseValue
	DROP CONSTRAINT FKCA47C816F8F076C0
GO
ALTER TABLE dbo.FormResponseCoords_ResponseValue
	DROP CONSTRAINT FKFA275C14699E17A8
GO
ALTER TABLE dbo.ResponseValue_ResponseValue
	DROP CONSTRAINT FK475D51213B0E124D
GO
ALTER TABLE dbo.ResponseValue_ResponseValue
	DROP CONSTRAINT FK475D51219AE2EA6
GO
DROP TABLE dbo.ResponseValue
GO
EXECUTE sp_rename N'dbo.Tmp_ResponseValue', N'ResponseValue', 'OBJECT' 
GO
ALTER TABLE dbo.ResponseValue ADD CONSTRAINT
	PK__Response__3213E83F29572725 PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ResponseValue ADD CONSTRAINT
	UQ__Response__3213E83E2C3393D0 UNIQUE NONCLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.ResponseValue', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.ResponseValue', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.ResponseValue', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.ResponseValue_ResponseValue ADD CONSTRAINT
	FK475D51213B0E124D FOREIGN KEY
	(
	repetableResponseValues_id
	) REFERENCES dbo.ResponseValue
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ResponseValue_ResponseValue ADD CONSTRAINT
	FK475D51219AE2EA6 FOREIGN KEY
	(
	ResponseValue_id
	) REFERENCES dbo.ResponseValue
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ResponseValue_ResponseValue SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.ResponseValue_ResponseValue', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.ResponseValue_ResponseValue', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.ResponseValue_ResponseValue', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.FormResponseCoords_ResponseValue ADD CONSTRAINT
	FKFA275C14699E17A8 FOREIGN KEY
	(
	formResponses_id
	) REFERENCES dbo.ResponseValue
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.FormResponseCoords_ResponseValue SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.FormResponseCoords_ResponseValue', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.FormResponseCoords_ResponseValue', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.FormResponseCoords_ResponseValue', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.FormResponse_ResponseValue ADD CONSTRAINT
	FKCA47C816F8F076C0 FOREIGN KEY
	(
	results_id
	) REFERENCES dbo.ResponseValue
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.FormResponse_ResponseValue SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.FormResponse_ResponseValue', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.FormResponse_ResponseValue', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.FormResponse_ResponseValue', 'Object', 'CONTROL') as Contr_Per 