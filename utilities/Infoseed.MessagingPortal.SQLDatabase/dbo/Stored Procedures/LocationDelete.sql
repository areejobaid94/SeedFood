-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE LocationDelete
(
  @LocationId int,
  @TenantId int
)
AS
BEGIN
   delete from [dbo].[LocationDeliveryCost] where LocationId =@LocationId and TenantId=@TenantId
END