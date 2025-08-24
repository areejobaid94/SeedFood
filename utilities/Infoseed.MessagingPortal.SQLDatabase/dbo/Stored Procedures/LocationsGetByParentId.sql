-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <25-4-2021>
-- Description:	<Get Locations By ParentId>
-- =============================================
CREATE PROCEDURE [dbo].[LocationsGetByParentId] 
	@ParentId int
AS
BEGIN
SELECT        Id, LocationName, LevelId, ParentId, GoogleURL, LocationNameEn
FROM            Locations
WHERE        (ParentId = @ParentId)END