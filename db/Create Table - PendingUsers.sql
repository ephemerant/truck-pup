USE [Trucks]
GO

/****** Object:  Table [dbo].[PendingUsers]    Script Date: 5/2/2017 4:02:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PendingUsers](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL
) ON [PRIMARY]

GO

