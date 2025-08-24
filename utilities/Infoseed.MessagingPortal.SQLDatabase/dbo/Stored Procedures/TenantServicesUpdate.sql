
-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <4-1-2021>
-- Description:	<Update Tenant Services>
-- =============================================
CREATE PROCEDURE [dbo].[TenantServicesUpdate] 
	@TenantId int,
	@Services nvarchar(256) --Comma Seperator 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	delete from TenantServices where TenantId = @TenantId and InfoSeedServiceId not in (select [value] from STRING_SPLIT(@Services,','))

	INSERT INTO [dbo].[TenantServices]
           ([CreationTime]
           ,[CreatorUserId]
           ,[LastModificationTime]
           ,[LastModifierUserId]
           ,[IsDeleted]
           ,[DeleterUserId]
           ,[DeletionTime]
           ,[TenantId]
           ,[ServiceFees]
           ,[InfoSeedServiceId])
	select 
	getdate()
	,1
	,NULL
	,NULL
	,0
	,NULL
	,NULL
	,@TenantId
	,InfoSeedServices.ServiceFees
	,InfoSeedServices.Id
	from STRING_SPLIT(@Services,',') 
	--left join TenantServices on TenantServices.InfoSeedServiceId =[value] 
	inner join InfoSeedServices on InfoSeedServices.Id = [value]
	--where TenantServices.Id is null

END