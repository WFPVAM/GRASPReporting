USE [GRASP]
GO

/****** Object:  Table [dbo].[UserToFormResponses]    Script Date: 11/07/2014 18:07:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserToFormResponses](
	[UserToFormResponsesID] [int] IDENTITY(1,1) NOT NULL,
	[userID] [int] NOT NULL,
	[formResponseID] [numeric](19, 0) NOT NULL,
	[formID] [numeric](19, 0) NOT NULL,
 CONSTRAINT [PK_UserToFormResponses] PRIMARY KEY CLUSTERED 
(
	[UserToFormResponsesID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

