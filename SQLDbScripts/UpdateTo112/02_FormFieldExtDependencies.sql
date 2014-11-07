USE [GRASP]
GO

/****** Object:  Table [dbo].[FormFieldExtDependencies]    Script Date: 11/07/2014 18:02:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FormFieldExtDependencies](
	[FFEDID] [int] IDENTITY(1,1) NOT NULL,
	[FormFieldID] [decimal](19, 0) NOT NULL,
	[FormFieldExtID] [int] NOT NULL,
	[FormFieldDefaultValue] [float] NULL,
 CONSTRAINT [PK_FormFieldExtDependencies] PRIMARY KEY CLUSTERED 
(
	[FFEDID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

