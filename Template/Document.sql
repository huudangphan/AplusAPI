USE [SBO_TKMB_GOLIVE]
GO

/****** Object:  Table [dbo].[@QCNVL]    Script Date: 4/14/2021 4:21:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[@QCNVL](
	[DocEntry] [int] NOT NULL,
	[DocNum] [int] NULL,
	[Period] [int] NULL,
	[Instance] [smallint] NULL,
	[Series] [int] NULL,
	[Handwrtten] [char](1) NULL,
	[Canceled] [char](1) NULL,
	[Object] [nvarchar](20) NULL,
	[UserSign] [int] NULL,
	[Transfered] [char](1) NULL,
	[Status] [char](1) NULL,
	[CreateDate] [datetime] NULL,
	[CreateTime] [smallint] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateTime] [smallint] NULL,
	[DataSource] [char](1) NULL,
	[RequestStatus] [char](1) NULL,
	[Creator] [nvarchar](8) NULL,
	[Remark] [ntext] NULL,
 CONSTRAINT [KQCNVL_PR] PRIMARY KEY CLUSTERED 
(
	[DocEntry] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[@QCNVL] ADD  CONSTRAINT [DF_@QCNVL_Instance]  DEFAULT ((0)) FOR [Instance]
GO

ALTER TABLE [dbo].[@QCNVL] ADD  CONSTRAINT [DF_@QCNVL_Handwrtten]  DEFAULT ('N') FOR [Handwrtten]
GO

ALTER TABLE [dbo].[@QCNVL] ADD  CONSTRAINT [DF_@QCNVL_Canceled]  DEFAULT ('N') FOR [Canceled]
GO

ALTER TABLE [dbo].[@QCNVL] ADD  CONSTRAINT [DF_@QCNVL_Transfered]  DEFAULT ('N') FOR [Transfered]
GO

ALTER TABLE [dbo].[@QCNVL] ADD  CONSTRAINT [DF_@QCNVL_Status]  DEFAULT ('O') FOR [Status]
GO

ALTER TABLE [dbo].[@QCNVL] ADD  CONSTRAINT [DF_@QCNVL_RequestStatus]  DEFAULT ('W') FOR [RequestStatus]
GO

ALTER TABLE [dbo].[@QCNVL] ADD  CONSTRAINT [DF_@QCNVL_U_Status]  DEFAULT ('D') FOR [U_Status]
GO


