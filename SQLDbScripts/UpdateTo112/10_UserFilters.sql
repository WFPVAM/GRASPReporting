USE [GRASP]
GO

/****** Object:  Table [dbo].[UserFilters]    Script Date: 11/14/2014 12:06:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserFilters](
	[UserFilterID] [int] IDENTITY(1,1) NOT NULL,
	[userID] [int] NOT NULL,
	[UserFilterString] [nvarchar](max) NULL,
	[formID] [numeric](19, 0) NOT NULL,
	[UserFilterCreateDate] [datetime] NOT NULL,
	[UserFilterIsEnabled] [int] NOT NULL,
	[UserFilterDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserFilters] PRIMARY KEY CLUSTERED 
(
	[UserFilterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

