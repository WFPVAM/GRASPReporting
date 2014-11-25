USE [GRASP]
GO

/****** Object:  View [dbo].[ResponseRepeatableReviews]    Script Date: 11/21/2014 16:54:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ResponseRepeatableReviews]'))
DROP VIEW [dbo].[ResponseRepeatableReviews]
GO

USE [GRASP]
GO

/****** Object:  View [dbo].[ResponseRepeatableReviews]    Script Date: 11/21/2014 16:54:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ResponseRepeatableReviews]
AS
SELECT     TOP (100) PERCENT dbo.FormFieldResponsesReviews.id, dbo.FormFieldResponsesReviews.value, dbo.FormFieldResponsesReviews.FormResponseID, 
                      dbo.FormFieldResponsesReviews.RVRepeatCount, dbo.FormFieldResponsesReviews.formFieldId, dbo.FormField_FormField.FormField_id AS ParentFormFieldID, 
                      dbo.FormFieldResponsesReviews.parentForm_id, dbo.FormFieldResponsesReviews.label, dbo.FormFieldResponsesReviews.name, 
                      dbo.FormFieldResponsesReviews.survey_id, dbo.FormFieldResponsesReviews.positionIndex, dbo.SurveyElement.positionIndex AS SurveyElementIndex, 
                      dbo.FormFieldResponsesReviews.type, dbo.FormFieldResponsesReviews.ResponseStatusID, dbo.FormFieldResponsesReviews.nvalue, 
                      dbo.FormFieldResponsesReviews.dvalue
FROM         dbo.Survey_SurveyElement INNER JOIN
                      dbo.Survey ON dbo.Survey_SurveyElement.Survey_id = dbo.Survey.id INNER JOIN
                      dbo.SurveyElement ON dbo.Survey_SurveyElement.values_id = dbo.SurveyElement.id RIGHT OUTER JOIN
                      dbo.FormFieldResponsesReviews ON dbo.SurveyElement.value = dbo.FormFieldResponsesReviews.value AND 
                      dbo.Survey.id = dbo.FormFieldResponsesReviews.survey_id LEFT OUTER JOIN
                      dbo.FormField_FormField ON dbo.FormFieldResponsesReviews.formFieldId = dbo.FormField_FormField.repetableFormFields_id
WHERE     (dbo.FormFieldResponsesReviews.RVRepeatCount <> 0)
ORDER BY dbo.FormFieldResponsesReviews.FormResponseID, dbo.FormFieldResponsesReviews.RVRepeatCount, dbo.FormFieldResponsesReviews.formFieldId

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[18] 4[4] 2[43] 3) )"
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
         Configuration = "(H (4 [30] 2 [40] 3))"
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
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Survey_SurveyElement"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 189
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Survey"
            Begin Extent = 
               Top = 6
               Left = 227
               Bottom = 99
               Right = 378
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SurveyElement"
            Begin Extent = 
               Top = 6
               Left = 416
               Bottom = 114
               Right = 567
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "FormFieldResponsesReviews"
            Begin Extent = 
               Top = 6
               Left = 605
               Bottom = 114
               Right = 774
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "FormField_FormField"
            Begin Extent = 
               Top = 102
               Left = 38
               Bottom = 195
               Right = 229
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 17
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
         Width = 1500
         Width = 1500
         Width = 15' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ResponseRepeatableReviews'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'00
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ResponseRepeatableReviews'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ResponseRepeatableReviews'
GO

