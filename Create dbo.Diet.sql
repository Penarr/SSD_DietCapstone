USE [SmartDietCapstone]
GO

/****** Object: Table [dbo].[Diet] Script Date: 2021-10-23 9:41:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Diet] (
    [DietId]         NVARCHAR (450) NOT NULL,
    [UserId]         NVARCHAR (450) NOT NULL,
    [SerializedDiet] TEXT           NOT NULL
);


