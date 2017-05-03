USE [Trucks]
GO

/****** Object:  Table [dbo].[Calendar]    Script Date: 5/2/2017 4:02:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Calendar](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[uid] [nvarchar](50) NOT NULL,
	[title] [nvarchar](200) NOT NULL,
	[start] [datetime] NOT NULL,
	[end] [datetime] NOT NULL,
	[increment] [smallint] NOT NULL,
	[increment_end] [datetime] NULL,
 CONSTRAINT [PK_Calendar] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [DF_Calendar_increment]  DEFAULT ((0)) FOR [increment]
GO

ALTER TABLE [dbo].[Calendar]  WITH CHECK ADD  CONSTRAINT [FK_Calendar_Users] FOREIGN KEY([uid])
REFERENCES [dbo].[Users] ([uid])
GO

ALTER TABLE [dbo].[Calendar] CHECK CONSTRAINT [FK_Calendar_Users]
GO

