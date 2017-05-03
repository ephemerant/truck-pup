USE [Trucks]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 5/2/2017 4:02:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[uid] [nvarchar](50) NOT NULL,
	[email] [nvarchar](100) NULL,
	[name] [nvarchar](100) NULL,
	[admin] [bit] NOT NULL,
	[vendor] [bit] NOT NULL,
	[broadcasting] [bit] NOT NULL,
	[enabled] [bit] NOT NULL,
	[food_type] [nvarchar](50) NULL,
	[about] [nvarchar](100) NULL,
	[logo] [nvarchar](200) NULL,
	[facebook] [nvarchar](200) NULL,
	[twitter] [nvarchar](200) NULL,
	[menu] [nvarchar](max) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_admin]  DEFAULT ((0)) FOR [admin]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_vendor]  DEFAULT ((0)) FOR [vendor]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_broadcasting]  DEFAULT ((0)) FOR [broadcasting]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_enabled]  DEFAULT ((1)) FOR [enabled]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_menu]  DEFAULT ('{"Combo":[],"Entree":[],"Side":[],"Drink":[]}') FOR [menu]
GO

