USE [GRASP]
GO

/****** Object:  Table [dbo].[FormResponseStatus]    Script Date: 11/07/2014 18:03:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FormResponseStatus](
	[ResponseStatusID] [int] IDENTITY(1,1) NOT NULL,
	[ResponseStatusName] [nvarchar](50) NULL,
	[ResponseStatusIndex] [int] NULL,
	[ResponseStatusDependency] [int] NULL,
 CONSTRAINT [PK_ResponseStatus] PRIMARY KEY CLUSTERED 
(
	[ResponseStatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

