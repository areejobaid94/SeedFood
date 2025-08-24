
-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <25-4-2021>
-- Description:	<GetLocations>
-- =============================================
CREATE PROCEDURE [dbo].[LocationsGet]
	 @TenantId int
	,@LocationId int =null
AS
BEGIN

SELECT       LocationDeliveryCost.BranchAreaId, LocationDeliveryCost.DeliveryCost, Locations.Id, Locations.LocationName, Locations.LevelId, Locations.ParentId, Locations.GoogleURL, Locations.LocationNameEn, Locations_1.LocationName AS AreaName,Locations_1.Id AS AreaId, 
                         Locations_2.LocationName AS CityName,Locations_2.Id AS CityId
FROM            LocationDeliveryCost INNER JOIN
                         Locations ON LocationDeliveryCost.LocationId = Locations.Id  AND LocationDeliveryCost.TenantId = @TenantId INNER JOIN
                         Locations AS Locations_1 ON Locations.ParentId = Locations_1.Id INNER JOIN
                         Locations AS Locations_2 ON Locations_1.ParentId = Locations_2.Id
						 where LocationDeliveryCost.LocationId in (select Id from Locations where ParentId = @LocationId)
END