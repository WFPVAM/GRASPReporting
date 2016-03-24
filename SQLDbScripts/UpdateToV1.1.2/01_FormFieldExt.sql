USE [GRASP]
GO

/****** Object:  Table [dbo].[FormFieldExt]    Script Date: 11/07/2014 18:02:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FormFieldExt](
	[FormFieldExtID] [int] IDENTITY(1,1) NOT NULL,
	[FormID] [numeric](19, 0) NOT NULL,
	[FormFieldExtFormula] [nvarchar](max) NULL,
	[FormFieldExtName] [nvarchar](50) NULL,
	[FormFieldExtLabel] [nvarchar](500) NULL,
	[PositionIndex] [int] NULL,
	[FormFieldID] [numeric](19, 0) NULL,
 CONSTRAINT [PK_FormFieldExt] PRIMARY KEY CLUSTERED 
(
	[FormFieldExtID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

