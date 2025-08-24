
GO
/****** Object:  Schema [Integration]    Script Date: 10/22/2020 11:32:34 AM ******/
CREATE SCHEMA [Integration]
GO
/****** Object:  Table [Integration].[ActionLogs]    Script Date: 10/22/2020 11:32:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Integration].[ActionLogs](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestorIPAddress] [nvarchar](max) NULL,
	[MachineName] [nvarchar](max) NULL,
	[UserName] [nvarchar](max) NULL,
	[RequestUri] [nvarchar](max) NULL,
	[RequestMethod] [nvarchar](20) NULL,
	[RequestTimestamp] [datetime] NULL,
	[RequestContentType] [nvarchar](100) NULL,
	[RequestHeaders] [nvarchar](max) NULL,
	[RequestContent] [nvarchar](max) NULL,
	[RequestRawData] [nvarchar](max) NULL,
	[ResponseStatusCode] [int] NULL,
	[ResponseTimestamp] [datetime] NULL,
	[ResponseContentType] [nvarchar](100) NULL,
	[ResponseHeaders] [nvarchar](max) NULL,
	[ResponseContent] [nvarchar](max) NULL,
	[ResponseRawData] [nvarchar](max) NULL,
 CONSTRAINT [PK_ActionLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [Integration].[ActionLogInsert]    Script Date: 10/22/2020 11:32:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Integration].[ActionLogInsert]
	@RequestorIPAddress nvarchar(max),
	@MachineName NVARCHAR(MAX),
	@UserName NVARCHAR(MAX) = NULL,
	@RequestUri nvarchar(max),
	@RequestMethod nvarchar(20),
	@RequestTimestamp datetime,
	@RequestContentType nvarchar(100),
	@RequestHeaders nvarchar(max) = NULL,
	@RequestContent nvarchar(max) = NULL,
	@RequestRawData nvarchar(max) = NULL,
	@ResponseStatusCode INT = NULL,
	@ResponseTimestamp DATETIME = NULL,
	@ResponseContentType nvarchar(100) = NULL,
	@ResponseHeaders nvarchar(max) = NULL,
	@ResponseContent nvarchar(max) = NULL,
	@ResponseRawData nvarchar(max) = NULL,
	@ID bigint OUTPUT
AS


INSERT INTO Integration.ActionLogs (
	[RequestorIPAddress],
	[MachineName],
	[UserName],
	[RequestUri],
	[RequestMethod],
	[RequestTimestamp],
	[RequestContentType],
	[RequestHeaders],
	[RequestContent],
	[RequestRawData],
	[ResponseStatusCode],
	[ResponseTimestamp],
	[ResponseContentType],
	[ResponseHeaders],
	[ResponseContent],
	[ResponseRawData]
) VALUES (
	@RequestorIPAddress,
	 @MachineName,
	 @UserName,
	@RequestUri,
	@RequestMethod,
	@RequestTimestamp,
	@RequestContentType,
	@RequestHeaders,
	@RequestContent,
	@RequestRawData,
	@ResponseStatusCode,
	@ResponseTimestamp,
	@ResponseContentType,
	@ResponseHeaders,
	@ResponseContent,
	@ResponseRawData
    
)

SET @ID = @@IDENTITY

GO
/****** Object:  StoredProcedure [Integration].[ActionLogUpdate]    Script Date: 10/22/2020 11:32:34 AM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Integration].[ActionLogUpdate]
	@ID bigint, 
	@RequestorIPAddress nvarchar(max), 
	@MachineName NVARCHAR(MAX),
	@UserName NVARCHAR(MAX) = NULL,
	@RequestUri nvarchar(max), 
	@RequestMethod nvarchar(20), 
	@RequestTimestamp datetime, 
	@RequestContentType nvarchar(100), 
	@RequestHeaders nvarchar(max) = NULL, 
	@RequestContent nvarchar(max) = NULL, 
	@RequestRawData nvarchar(max) = NULL, 
	@ResponseStatusCode INT =NULL, 
	@ResponseTimestamp datetime, 
	@ResponseContentType nvarchar(100), 
	@ResponseHeaders nvarchar(max) = NULL, 
	@ResponseContent nvarchar(max) = NULL, 
	@ResponseRawData nvarchar(max) = NULL 
AS

UPDATE Integration.ActionLogs SET
	[RequestorIPAddress] = @RequestorIPAddress,
	[MachineName] = @MachineName,
	[UserName] = @UserName,
	[RequestUri] = @RequestUri,
	[RequestMethod] = @RequestMethod,
	[RequestTimestamp] = @RequestTimestamp,
	[RequestContentType] = @RequestContentType,
	[RequestHeaders] = @RequestHeaders,
	[RequestContent] = @RequestContent,
	[RequestRawData] = @RequestRawData,
	[ResponseStatusCode] = @ResponseStatusCode,
	[ResponseTimestamp] = @ResponseTimestamp,
	[ResponseContentType] = @ResponseContentType,
	[ResponseHeaders] = @ResponseHeaders,
	[ResponseContent] = @ResponseContent,
	[ResponseRawData] = @ResponseRawData
	
WHERE
	[ID] = @ID


GO
