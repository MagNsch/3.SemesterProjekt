USE [DMA-CSD-V221_10434660]

CREATE TABLE [dbo].[Painting](
	Id int IDENTITY(1,1) NOT NULL,
	Title nvarchar(50) NOT NULL,
	Price decimal(38, 2) NULL,
	Stock int NOT NULL,
	Artist nvarchar(50) NULL,
	[Description] nvarchar(250) NULL,
	Category nvarchar(20) NULL,
	PRIMARY KEY (Id)
	)