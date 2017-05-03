USE [Trucks]
GO

/****** Object:  Table [dbo].[Locations]    Script Date: 5/2/2017 4:02:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Locations](
	[uid] [nvarchar](50) NOT NULL,
	[lat] [decimal](10, 8) NOT NULL,
	[lon] [decimal](10, 8) NOT NULL,
	[time] [datetime] NOT NULL,
	[hours] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Locations] ADD  CONSTRAINT [DF_Locations_time]  DEFAULT (getdate()) FOR [time]
GO

ALTER TABLE [dbo].[Locations] ADD  CONSTRAINT [DF_Locations_hours]  DEFAULT ((4)) FOR [hours]
GO

ALTER TABLE [dbo].[Locations]  WITH CHECK ADD  CONSTRAINT [FK_Locations_Users] FOREIGN KEY([uid])
REFERENCES [dbo].[Users] ([uid])
GO

ALTER TABLE [dbo].[Locations] CHECK CONSTRAINT [FK_Locations_Users]
GO

