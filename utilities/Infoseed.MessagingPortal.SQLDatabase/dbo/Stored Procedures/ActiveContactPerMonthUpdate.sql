-- =============================================
-- Author:		<Mustafa Snaid>
-- Create date: <2-4-2021>
-- Description:	<ActiveContactPerMonthUpdate>
-- =============================================
CREATE PROCEDURE [dbo].[ActiveContactPerMonthUpdate] 
@UserId nvarchar(250),
@MessageDate datetime
AS
BEGIN
	declare @contactId int,
	@tenantId int

	select @contactId=id,@tenantId=TenantId from Contacts where UserId = @UserId

	if(@tenantId is not null and @contactId is not null)
	begin

	if exists (select * from [dbo].[ActiveContactPerMonth] where month(@MessageDate) = [Month] and year(@MessageDate) = [Year] and ContactId = @contactId)
begin
update [dbo].[ActiveContactPerMonth] 
set LastMessageDateTime = @MessageDate
where ContactId = @contactId and year(LastMessageDateTime) = [Year] and month(@MessageDate) = [Month] 

end
else
begin

	INSERT INTO [dbo].[ActiveContactPerMonth]
           ([TenantId]
           ,[Year]
           ,[Month]
           ,[LastMessageDateTime]
           ,[ContactID])
     VALUES
           (@tenantId
           ,year(@MessageDate)
           ,month(@MessageDate)
           ,@MessageDate
           ,@contactId)

		   end
		   end

END