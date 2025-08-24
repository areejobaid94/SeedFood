-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <25-4-2021>
-- Description:	<Add Location Delivery Cost>
-- =============================================
CREATE PROCEDURE [dbo].[LocationAdd] 
	@LocationId int,
	@DeliveryCost decimal(18,3),
	@TenantId int,
	@BranchAreaId int
AS
BEGIN
if not exists(select * from LocationDeliveryCost where [TenantId] = @TenantId and [LocationId] = @LocationId)
begin
insert into LocationDeliveryCost 
values(@TenantId,@LocationId,@DeliveryCost,@BranchAreaId)
end
else
begin
update LocationDeliveryCost
	set DeliveryCost = @DeliveryCost ,	BranchAreaId = @BranchAreaId
	where [TenantId] = @TenantId and [LocationId] = @LocationId
	end
END