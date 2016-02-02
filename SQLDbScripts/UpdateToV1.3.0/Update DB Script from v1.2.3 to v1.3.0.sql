USE [GRASP]
GO

/****** 1_AddColumnsToFormTable ******/
Alter table [GRASP].[dbo].[Form] add 
	[IsDeleted] [tinyint]  NULL,
	[DeletedDate] [datetime] NULL,
	[FormVersion] [tinyint] NULL,
	[PreviousPublishedName] [nvarchar](100) NULL,
	[PreviousPublishedID] [nvarchar](100) NULL;

/****** 2_Update FormFieldResponses View ******/	

/****** Object:  View [dbo].[FormFieldResponses]    Script Date: 9/28/2015 1:56:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[FormFieldResponses]
AS
SELECT        dbo.ResponseValue.id, dbo.ResponseValue.value, dbo.ResponseValue.RVRepeatCount, dbo.ResponseValue.FormResponseID, dbo.ResponseValue.RVCreateDate, 
                         dbo.ResponseValue.formFieldId, dbo.FormResponse.parentForm_id, dbo.FormField.label, dbo.FormField.name, dbo.FormField.type, dbo.FormField.survey_id, 
                         dbo.FormField.positionIndex, dbo.FormResponse.senderMsisdn, dbo.FormResponse.FRCreateDate, FormResponse.ResponseStatusID, dbo.ResponseValue.nvalue, 
                         dbo.ResponseValue.dvalue
FROM            dbo.ResponseValue INNER JOIN
                         dbo.FormField ON dbo.ResponseValue.formFieldId = dbo.FormField.id INNER JOIN
                         dbo.FormResponse ON dbo.ResponseValue.FormResponseID = dbo.FormResponse.id
where FormResponse.ResponseStatusID <> 5
UNION
SELECT        respValueExtID AS id, CAST(nvalue AS nvarchar(255)) AS value, 0 AS rvRepeatCount, FormResponseID AS FormResponseID, NULL AS RVCreateDate, 
                         ResponseValueExt.formfieldID AS formFieldId, formID AS ParentForm_ID, FormFieldExtLabel AS label, FormFieldExtName AS name, 
                         'SERVERSIDE-CALCULATED' AS type, NULL AS survey_id, ResponseValueExt.positionindex, senderMSISDN, FRCreateDate, FormResponse.ResponseStatusID, 
                         nvalue AS nvalue, NULL AS dvalue
FROM            ResponseValueExt INNER JOIN
                         FormFieldExt ON ResponseValueExt.FormFieldExtID = FormFieldExt.FormFieldExtID INNER JOIN
                         FormResponse ON ResponseValueExt.FormResponseID = FormResponse.id

GO


/****** 3_CreatePermissionsTable ******/
USE [GRASP]
GO

/****** Object:  Table [dbo].[Permissions]    Script Date: 10/06/2015 09:56:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Permissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** 4_CreateRole_PermissionsTable ******/
USE [GRASP]
GO

/****** Object:  Table [dbo].[Role_Permissions]    Script Date: 10/06/2015 09:57:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Role_Permissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
 CONSTRAINT [PK_Role_Permissions_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Role_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_Role_Permissions_Permissions] FOREIGN KEY([PermissionID])
REFERENCES [dbo].[Permissions] ([id])
GO

ALTER TABLE [dbo].[Role_Permissions] CHECK CONSTRAINT [FK_Role_Permissions_Permissions]
GO

ALTER TABLE [dbo].[Role_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_Role_Permissions_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([id])
GO

ALTER TABLE [dbo].[Role_Permissions] CHECK CONSTRAINT [FK_Role_Permissions_Roles]
GO


/****** 5_InsertPermissionsAndRolePermissions ******/
USE [GRASP]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 10/06/2015 10:01:58 ******/
--SET IDENTITY_INSERT [dbo].[Permissions] ON
INSERT [dbo].[Permissions] ([Name], [Description]) VALUES (N'EditFormResponse', N'Edit Form Response')
INSERT [dbo].[Permissions] ([Name], [Description]) VALUES (N'DeleteFormResponse', N'Delete Form Response')
INSERT [dbo].[Permissions] ([Name], [Description]) VALUES (N'SeePermissionsSection', N'See Permissions Section')
--SET IDENTITY_INSERT [dbo].[Permissions] OFF
/****** Object:  Table [dbo].[Role_Permissions]    Script Date: 10/06/2015 10:01:58 ******/
--SET IDENTITY_INSERT [dbo].[Role_Permissions] ON
INSERT [dbo].[Role_Permissions] ([RoleID], [PermissionID]) VALUES (3, 1)
INSERT [dbo].[Role_Permissions] ([RoleID], [PermissionID]) VALUES (3, 3)
--SET IDENTITY_INSERT [dbo].[Role_Permissions] OFF

/****** 6_AddColumnToFormResponseTable ******/
Alter table [GRASP].[dbo].[FormResponse] add [LastUpdatedDate] [datetime] NULL


UPDATE [dbo].[Form] set FormVersion=1 WHERE FormVersion IS NULL
UPDATE [dbo].[Form] set IsDeleted=0 WHERE IsDeleted IS NULL;


/* alter form respone **/

ALTER TABLE [FormResponse] DROP CONSTRAINT [DF_FormResponse_ResponseStatusID];

ALTER TABLE [FormResponse] ADD CONSTRAINT DF_FormResponse_ResponseStatusID DEFAULT 1 FOR [ResponseStatusID];