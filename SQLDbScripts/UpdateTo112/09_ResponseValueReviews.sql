USE [GRASP]
GO

/****** Object:  Table [dbo].[ResponseValueReviews]    Script Date: 11/07/2014 18:06:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ResponseValueReviews](
	[id] [numeric](19, 0) IDENTITY(1,1) NOT NULL,
	[value] [nvarchar](255) NULL,
	[RVRepeatCount] [int] NULL,
	[FormResponseID] [int] NULL,
	[formFieldId] [int] NULL,
	[positionIndex] [int] NULL,
	[nvalue] [float] NULL,
	[dvalue] [datetime] NULL,
	[FormResponseReviewID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

