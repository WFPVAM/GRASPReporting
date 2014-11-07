USE [GRASP_UNDP4]
GO

/****** Object:  View [dbo].[FormFieldExport]    Script Date: 11/07/2014 18:46:10 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[FormFieldExport]'))
DROP VIEW [dbo].[FormFieldExport]
GO

USE [GRASP_UNDP4]
GO

/****** Object:  View [dbo].[FormFieldExport]    Script Date: 11/07/2014 18:46:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[FormFieldExport]
AS
SELECT     dbo.FormField.id, dbo.FormField.label, dbo.FormField.name, dbo.FormField.positionIndex, dbo.FormField.required, dbo.FormField.type, dbo.FormField.x_form, dbo.FormField.form_id, 
                      dbo.FormField.survey_id, dbo.FormField.isReadOnly, dbo.FormField.numberOfRep, dbo.FormField.FFCreateDate, dbo.FormField_FormField.FormField_id AS FormFieldParentID, 
                      dbo.FormField.calculated, dbo.FormField.formula

FROM         dbo.FormField LEFT OUTER JOIN dbo.FormField_FormField ON dbo.FormField.id = dbo.FormField_FormField.repetableFormFields_id

GO

