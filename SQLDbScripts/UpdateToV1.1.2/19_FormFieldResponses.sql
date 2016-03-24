USE [GRASP]
GO

/****** Object:  View [dbo].[FormFieldResponses]    Script Date: 11/07/2014 19:08:20 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[FormFieldResponses]'))
DROP VIEW [dbo].[FormFieldResponses]
GO

USE [GRASP]
GO

/****** Object:  View [dbo].[FormFieldResponses]    Script Date: 11/07/2014 19:08:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[FormFieldResponses]
AS
SELECT     dbo.ResponseValue.id, dbo.ResponseValue.value, dbo.ResponseValue.RVRepeatCount, dbo.ResponseValue.FormResponseID, dbo.ResponseValue.RVCreateDate, 
                      dbo.ResponseValue.formFieldId, dbo.FormResponse.parentForm_id, dbo.FormField.label, dbo.FormField.name, dbo.FormField.type, dbo.FormField.survey_id, 
                      dbo.FormField.positionIndex, dbo.FormResponse.senderMsisdn, dbo.FormResponse.FRCreateDate, FormResponse.ResponseStatusID, 
                      dbo.ResponseValue.nvalue, dbo.ResponseValue.dvalue
FROM         dbo.ResponseValue INNER JOIN
                      dbo.FormField ON dbo.ResponseValue.formFieldId = dbo.FormField.id INNER JOIN
                      dbo.FormResponse ON dbo.ResponseValue.FormResponseID = dbo.FormResponse.id 
UNION
SELECT     respValueExtID AS id, CAST(nvalue AS nvarchar(255)) AS value, 0 AS rvRepeatCount, FormResponseID AS FormResponseID, NULL AS RVCreateDate, 
                      ResponseValueExt.formfieldID AS formFieldId, formID AS ParentForm_ID, FormFieldExtLabel AS label, FormFieldExtName AS name, 
                      'SERVERSIDE-CALCULATED' AS type, NULL AS survey_id, ResponseValueExt.positionindex, senderMSISDN, FRCreateDate, FormResponse.ResponseStatusID, 
                      nvalue AS nvalue, NULL AS dvalue
FROM         ResponseValueExt INNER JOIN
                      FormFieldExt ON ResponseValueExt.FormFieldExtID = FormFieldExt.FormFieldExtID INNER JOIN
                      FormResponse ON ResponseValueExt.FormResponseID = FormResponse.id


GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[42] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4[30] 2[41] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 3
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'FormFieldResponses'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'FormFieldResponses'
GO

