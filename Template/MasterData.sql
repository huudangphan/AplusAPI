USE [SBO_TKMB_GOLIVE]
GO

/****** Object:  Table [dbo].[@ITEMLOG]    Script Date: 4/14/2021 4:20:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[@ITEMLOG](
	[Code] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[DocEntry] [int] NOT NULL,
	[Canceled] [char](1) NULL,
	[Object] [nvarchar](20) NULL,
	[UserSign] [int] NULL,
	[Transfered] [char](1) NULL,
	[CreateDate] [datetime] NULL,
	[CreateTime] [smallint] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateTime] [smallint] NULL,
	[DataSource] [char](1) NULL,
 CONSTRAINT [KITEMLOG_PR] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[@ITEMLOG] ADD  CONSTRAINT [DF_@ITEMLOG_Canceled]  DEFAULT ('N') FOR [Canceled]
GO

ALTER TABLE [dbo].[@ITEMLOG] ADD  CONSTRAINT [DF_@ITEMLOG_Transfered]  DEFAULT ('N') FOR [Transfered]
GO


