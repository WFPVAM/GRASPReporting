USE [GRASP]
GO

/****** Object:  Table [dbo].[FormResponseServerStatus]    Script Date: 02/26/2015 11:20:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FormResponseServerStatus](
	[Id] [numeric](19, 0) IDENTITY(1,1) NOT NULL,
	[InstanceUniqueIdentifier] [nvarchar](255) NOT NULL,
	[IsSavedToServer] [bit] NOT NULL,
 CONSTRAINT [PK_FormResponseServerStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Index [IX_FormResponseServerStatus_InstanceUniqueIdentifier]    Script Date: 1/16/2015 7:07:46 PM ******/
CREATE NONCLUSTERED INDEX [IX_FormResponseServerStatus_InstanceUniqueIdentifier] ON [dbo].[FormResponseServerStatus]
(
	[InstanceUniqueIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


