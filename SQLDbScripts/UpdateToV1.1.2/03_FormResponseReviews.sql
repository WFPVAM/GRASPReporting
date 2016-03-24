USE [GRASP]
GO

/****** Object:  Table [dbo].[FormResponseReviews]    Script Date: 11/07/2014 18:03:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FormResponseReviews](
	[FormResponseReviewID] [int] IDENTITY(1,1) NOT NULL,
	[FormResponseID] [numeric](19, 0) NOT NULL,
	[FRRUserName] [nvarchar](255) NULL,
	[FormResponseReviewDate] [datetime] NOT NULL,
	[FormResponsePreviousStatusID] [int] NOT NULL,
	[FormResponseCurrentStatusID] [int] NOT NULL,
	[FormResponseReviewDetail] [nvarchar](max) NULL,
	[FormResponseReviewSeqNo] [int] NULL,
 CONSTRAINT [PK_FormResponseReviews] PRIMARY KEY CLUSTERED 
(
	[FormResponseReviewID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

