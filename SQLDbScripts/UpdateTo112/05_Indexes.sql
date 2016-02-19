USE [GRASP]
GO

/****** Object:  Table [dbo].[Indexes]    Script Date: 11/07/2014 18:03:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Indexes](
	[IndexID] [int] IDENTITY(1,1) NOT NULL,
	[IndexName] [nvarchar](50) NULL,
	[IndexCreateDate] [datetime] NULL,
	[formID] [int] NOT NULL,
	[IndexLastUpdateDate] [datetime] NULL,
	[IndexLastUpdateUserName] [nvarchar](150) NULL,
	[IndexAlgorithm] [nvarchar](50) NULL,
 CONSTRAINT [PK_Indexes] PRIMARY KEY CLUSTERED 
(
	[IndexID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

