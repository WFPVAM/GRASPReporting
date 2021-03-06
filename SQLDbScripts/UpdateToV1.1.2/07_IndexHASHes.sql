USE [GRASP]
GO

/****** Object:  Table [dbo].[IndexHASHes]    Script Date: 11/07/2014 18:04:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IndexHASHes](
	[IndexHASHID] [int] IDENTITY(1,1) NOT NULL,
	[IndexHASHString] [nvarchar](250) NULL,
	[IndexID] [int] NOT NULL,
	[FormResponseID] [int] NOT NULL,
 CONSTRAINT [PK_IndexHASHes] PRIMARY KEY CLUSTERED 
(
	[IndexHASHID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

