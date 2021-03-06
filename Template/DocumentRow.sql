USE [SBO_TKMB_GOLIVE]
GO

/****** Object:  Table [dbo].[@QCNVL1]    Script Date: 4/14/2021 4:21:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[@QCNVL1](
	[DocEntry] [int] NOT NULL,
	[LineId] [int] NOT NULL,
	[VisOrder] [int] NULL,
	[Object] [nvarchar](20) NULL,
 CONSTRAINT [KQCNVL1_PR] PRIMARY KEY CLUSTERED 
(
	[DocEntry] ASC,
	[LineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[@QCNVL1] ADD  CONSTRAINT [DF_@QCNVL1_U_COA]  DEFAULT ('Y') FOR [U_COA]
GO


