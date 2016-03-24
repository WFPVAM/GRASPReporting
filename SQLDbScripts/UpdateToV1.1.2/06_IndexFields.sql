USE [GRASP]
GO

/****** Object:  Table [dbo].[IndexFields]    Script Date: 11/07/2014 18:04:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IndexFields](
	[IndexFieldID] [int] IDENTITY(1,1) NOT NULL,
	[IndexID] [int] NOT NULL,
	[FormFieldID] [numeric](19, 0) NOT NULL,
 CONSTRAINT [PK_IndexFields] PRIMARY KEY CLUSTERED 
(
	[IndexFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[IndexFields]  WITH CHECK ADD  CONSTRAINT [FK_IndexFields_FormField] FOREIGN KEY([FormFieldID])
REFERENCES [dbo].[FormField] ([id])
GO

ALTER TABLE [dbo].[IndexFields] CHECK CONSTRAINT [FK_IndexFields_FormField]
GO

ALTER TABLE [dbo].[IndexFields]  WITH CHECK ADD  CONSTRAINT [FK_IndexFields_IndexFields] FOREIGN KEY([IndexFieldID])
REFERENCES [dbo].[IndexFields] ([IndexFieldID])
GO

ALTER TABLE [dbo].[IndexFields] CHECK CONSTRAINT [FK_IndexFields_IndexFields]
GO

