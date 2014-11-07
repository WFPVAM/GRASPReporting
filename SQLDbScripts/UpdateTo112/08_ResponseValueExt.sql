USE [GRASP]
GO

/****** Object:  Table [dbo].[ResponseValueExt]    Script Date: 11/07/2014 18:06:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ResponseValueExt](
	[RespValueExtID] [int] IDENTITY(1,1) NOT NULL,
	[FormResponseID] [int] NOT NULL,
	[FormFieldExtID] [int] NOT NULL,
	[nvalue] [float] NULL,
	[FormFieldID] [int] NULL,
	[PositionIndex] [int] NULL,
 CONSTRAINT [PK_ResponseValueExt] PRIMARY KEY CLUSTERED 
(
	[RespValueExtID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

