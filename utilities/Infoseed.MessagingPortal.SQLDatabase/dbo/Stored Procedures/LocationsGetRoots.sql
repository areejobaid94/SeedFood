-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <25-4-2021>
-- Description:	<Get Root Locations>
-- =============================================
CREATE PROCEDURE [dbo].[LocationsGetRoots]
	
AS
BEGIN
	SELECT        Id, LocationName, LevelId, ParentId, GoogleURL, LocationNameEn
FROM            Locations
WHERE        (ParentId IS NULL)
END