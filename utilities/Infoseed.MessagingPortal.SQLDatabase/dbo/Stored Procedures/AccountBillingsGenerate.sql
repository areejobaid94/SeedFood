-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <1-1-2012>
-- Description:	<Account Billings Generation>
-- =============================================
CREATE PROCEDURE [dbo].[AccountBillingsGenerate] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

--- Handing One Time Service Frequency 
INSERT INTO [dbo].[AccountBillings]
           ([CreationTime]
           ,[CreatorUserId]
           ,[LastModificationTime]
           ,[LastModifierUserId]
           ,[IsDeleted]
           ,[DeleterUserId]
           ,[DeletionTime]
           ,[TenantId]
           ,[BillID]
           ,[BillDateFrom]
           ,[BillDateTo]
           ,[OpenAmount]
           ,[BillAmount]
           ,[InfoSeedServiceId]
           ,[ServiceTypeId]
           ,[CurrencyId])
    
SELECT  
GETDATE()
,1
,NULL
,NULL
,0
,NULL
,NULL
, TenantServices.TenantId
,CAST(TenantServices.TenantId as varchar) +'-' + format(GETDATE(),'yyyyMMdd') 
,GETDATE()
,DATEADD(year, 10, GETDATE())
,TenantServices.ServiceFees
,TenantServices.ServiceFees
, TenantServices.InfoSeedServiceId
, InfoSeedServices.ServiceTypeId
,1
--, TenantServices.Id
--, TenantServices.ServiceId

--, TenantServices.ServiceFees
--, TenantServices.CreationTime
--, TenantServices.IsDeleted
--, InfoSeedServices.ServiceFrquencyId
--, InfoSeedServices.ServiceName
--, InfoSeedServices.ServiceTypeId
--, ServiceTypes.ServicetypeName
--, ServiceFrquencies.ServiceFrequencyName
FROM            TenantServices INNER JOIN
                         InfoSeedServices ON TenantServices.InfoSeedServiceId = InfoSeedServices.Id INNER JOIN
                         ServiceTypes ON InfoSeedServices.ServiceTypeId = ServiceTypes.Id INNER JOIN
                         ServiceFrquencies ON InfoSeedServices.ServiceFrquencyId = ServiceFrquencies.Id 
						 left JOIN   AccountBillings ON TenantServices.TenantId = AccountBillings.TenantId AND TenantServices.InfoSeedServiceId = AccountBillings.InfoSeedServiceId
WHERE        (TenantServices.IsDeleted = 0) AND (InfoSeedServices.ServiceFrquencyId = 1) 
and (AccountBillings.Id is null)


-- Handling monthly services
INSERT INTO [dbo].[AccountBillings]
           ([CreationTime]
           ,[CreatorUserId]
           ,[LastModificationTime]
           ,[LastModifierUserId]
           ,[IsDeleted]
           ,[DeleterUserId]
           ,[DeletionTime]
           ,[TenantId]
           ,[BillID]
           ,[BillDateFrom]
           ,[BillDateTo]
           ,[OpenAmount]
           ,[BillAmount]
           ,[InfoSeedServiceId]
           ,[ServiceTypeId]
           ,[CurrencyId])
SELECT  
GETDATE()
,1
,NULL
,NULL
,0
,NULL
,NULL
, TenantServices.TenantId
,CAST(TenantServices.TenantId as varchar) +'-' + format(GETDATE(),'yyyyMMdd') 
,case
when AccountBillings.Id is null then TenantServices.CreationTime
else AccountBillings.BillDateTo end --
,case 
when AccountBillings.Id is null and InfoSeedServices.ServiceFrquencyId = 3  then DATEADD(month,1,TenantServices.CreationTime)
when AccountBillings.Id is not null and InfoSeedServices.ServiceFrquencyId = 2  then DATEADD(month,4,AccountBillings.BillDateTo)
when AccountBillings.Id is null and InfoSeedServices.ServiceFrquencyId = 2  then DATEADD(month,4,TenantServices.CreationTime)
else  dateadd(month,1, AccountBillings.BillDateTo) end
,TenantServices.ServiceFees
,TenantServices.ServiceFees
, TenantServices.InfoSeedServiceId
, InfoSeedServices.ServiceTypeId
,1
FROM            TenantServices INNER JOIN
                         InfoSeedServices ON TenantServices.InfoSeedServiceId = InfoSeedServices.Id INNER JOIN
                         ServiceTypes ON InfoSeedServices.ServiceTypeId = ServiceTypes.Id INNER JOIN
                         ServiceFrquencies ON InfoSeedServices.ServiceFrquencyId = ServiceFrquencies.Id 
						 left JOIN   AccountBillings ON TenantServices.TenantId = AccountBillings.TenantId AND TenantServices.InfoSeedServiceId = AccountBillings.InfoSeedServiceId
WHERE        (TenantServices.IsDeleted = 0) 
AND (InfoSeedServices.ServiceFrquencyId <> 1) 
and (AccountBillings.Id = (
select max(Id) from AccountBillings as AB 
where AB.TenantId = AccountBillings.TenantId 
and AB.InfoSeedServiceId = AccountBillings.InfoSeedServiceId)
or AccountBillings.Id is null)

and 

(case
when AccountBillings.Id is null then TenantServices.CreationTime
else  AccountBillings.BillDateTo end) <= getdate()

--and AccountBillings.BillDateTo <> AccountBillings.BillDateFrom

END