-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <19-1-2021>
-- Description:	<GetTenants>
-- =============================================
CREATE PROCEDURE [dbo].[TenantsGet] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   select * from  AbpTenants
END