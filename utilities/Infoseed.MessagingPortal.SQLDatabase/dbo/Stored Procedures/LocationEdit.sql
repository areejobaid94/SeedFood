-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <25-4-2021>
-- Description:	<Edit Location Delivery Cost>
-- =============================================
CREATE PROCEDURE [dbo].[LocationEdit] 
	@LocationId int,
	@DeliveryCost decimal(18,3),
	@TenantId int,
	@BranchAreaId int
AS
BEGIN
	update LocationDeliveryCost
	set DeliveryCost = @DeliveryCost,	BranchAreaId = @BranchAreaId
	where [TenantId] = @TenantId and [LocationId] = @LocationId
END