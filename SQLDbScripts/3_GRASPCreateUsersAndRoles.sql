USE [GRASP]
SET IDENTITY_INSERT [dbo].[Roles] ON
INSERT [dbo].[Roles] ([id], [description]) VALUES (1, N'ManagementToolAdministrator')
INSERT [dbo].[Roles] ([id], [description]) VALUES (2, N'DesignerToolAdministrator')
INSERT [dbo].[Roles] ([id], [description]) VALUES (3, N'SuperAdministrator')
INSERT [dbo].[Roles] ([id], [description]) VALUES (4, N'Supervisor')
INSERT [dbo].[Roles] ([id], [description]) VALUES (5, N'Designer')
INSERT [dbo].[Roles] ([id], [description]) VALUES (6, N'Guest')
INSERT [dbo].[Roles] ([id], [description]) VALUES (7, N'DataEntryOperator')
INSERT [dbo].[Roles] ([id], [description]) VALUES (8, N'Analyst')
INSERT [dbo].[Roles] ([id], [description]) VALUES (9, N'Reviewer')
INSERT [dbo].[Roles] ([id], [description]) VALUES (10, N'Reader')
INSERT [dbo].[Roles] ([id], [description]) VALUES (11, N'Reviewer2')
SET IDENTITY_INSERT [dbo].[Roles] OFF

SET IDENTITY_INSERT [dbo].[User_Credential] ON
INSERT [dbo].[User_Credential] ([user_id], [email], [frontlinesms_id], [name], [password], [phone_number], [pushed], [supervisor], [surname], [username], [roles_id], [UserDeleteDate]) VALUES (1, N'admin@WFP.it', N'AAABQ5uil5U', N'admin', N'admin', N'+3900000000', NULL, N'SuperAdministrator', N'admin', N'admin', 3, NULL)
INSERT [dbo].[User_Credential] ([user_id], [email], [frontlinesms_id], [name], [password], [phone_number], [pushed], [supervisor], [surname], [username], [roles_id], [UserDeleteDate]) VALUES (2, N'supervisor@WFP.org', NULL, N'Supervisor', N'grasp1', N'+3900000000', NULL, N'Supervisor', N'User', N'supervisor1', 4, NULL)
SET IDENTITY_INSERT [dbo].[User_Credential] OFF

INSERT INTO dbo.FormResponseStatus (ResponseStatusName,ResponseStatusIndex,ResponseStatusDependency) VALUES ('To Be Reviewed',1,0);
INSERT INTO dbo.FormResponseStatus (ResponseStatusName,ResponseStatusIndex,ResponseStatusDependency) VALUES ('Accepted',3,2);
INSERT INTO dbo.FormResponseStatus (ResponseStatusName,ResponseStatusIndex,ResponseStatusDependency) VALUES ('Rejected',4,0);
INSERT INTO dbo.FormResponseStatus (ResponseStatusName,ResponseStatusIndex,ResponseStatusDependency) VALUES ('Reviewed',2,1);
INSERT INTO dbo.FormResponseStatus (ResponseStatusName,ResponseStatusIndex,ResponseStatusDependency) VALUES ('Deleted',4,0);
INSERT INTO dbo.FormResponseStatus (ResponseStatusName,ResponseStatusIndex,ResponseStatusDependency) VALUES ('Pending',2,1);

--Insert Permissions
INSERT [dbo].[Permissions] ([Name], [Description]) VALUES (N'EditFormResponse', N'Edit Form Response')
INSERT [dbo].[Permissions] ([Name], [Description]) VALUES (N'DeleteFormResponse', N'Delete Form Response')
INSERT [dbo].[Permissions] ([Name], [Description]) VALUES (N'SeePermissionsSection', N'See Permissions Section')

--Insert default permission to enable edit responses for super administrator
--Give the superAdministrators to see the Permissions section
INSERT [dbo].[Role_Permissions] ([RoleID], [PermissionID]) VALUES (3, 1)
INSERT [dbo].[Role_Permissions] ([RoleID], [PermissionID]) VALUES (3, 3)
