-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ContactsAdd]
	-- Add the parameters for the stored procedure here
	 
	  @TenantId int 
	  ,@AvatarUrl  nvarchar(512)=null
      ,@DisplayName  nvarchar(512)
      ,@PhoneNumber  nvarchar(512) =null
      ,@SunshineAppID  nvarchar(512)
      ,@IsLockedByAgent bit
      ,@LockedByAgentName nvarchar(512) =null
      ,@IsOpen bit
      ,@Website nvarchar(512) =null
      ,@EmailAddress nvarchar(512) =null
      ,@Description nvarchar(max) =null
      ,@ChatStatuseId int =2
      ,@ContactStatuseId int =2
      ,@CreationTime datetime2(7)
      ,@CreatorUserId bigint
      ,@DeleterUserId bigint=null
      ,@DeletionTime  datetime2(7) =null
      ,@IsDeleted bit=null
      ,@LastModificationTime datetime2(7) =null
      ,@LastModifierUserId bigint= null
      ,@UserId nvarchar(max)
      ,@IsConversationExpired bit
      ,@IsBlock bit
	 
AS
BEGIN
	INSERT INTO [dbo].[Contacts]
           ([TenantId]
           ,[AvatarUrl]
           ,[DisplayName]
           ,[PhoneNumber]
           ,[SunshineAppID]
           ,[IsLockedByAgent]
           ,[LockedByAgentName]
           ,[IsOpen]
           ,[Website]
           ,[EmailAddress]
           ,[Description]
           ,[ChatStatuseId]
           ,[ContactStatuseId]
           ,[CreationTime]
           ,[CreatorUserId]
           ,[DeleterUserId]
           ,[DeletionTime]
           ,[IsDeleted]
           ,[LastModificationTime]
           ,[LastModifierUserId]
           ,[UserId]
           ,[IsConversationExpired]
           ,[IsBlock])
     VALUES
           (@TenantId
            ,@AvatarUrl  
            ,@DisplayName 
            ,@PhoneNumber 
            ,@SunshineAppID  
            ,@IsLockedByAgent 
            ,@LockedByAgentName 
            ,@IsOpen
            ,@Website 
            ,@EmailAddress
            ,@Description 
            ,@ChatStatuseId 
            ,@ContactStatuseId 
            ,@CreationTime
            ,@CreatorUserId 
            ,@DeleterUserId
            ,@DeletionTime  
            ,@IsDeleted 
            ,@LastModificationTime 
            ,@LastModifierUserId 
            ,@UserId 
            ,@IsConversationExpired
            ,@IsBlock )
END