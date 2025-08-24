using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class TenantServicesGet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"-- =============================================
                    -- Author:		<Mustafa Snaid>
                    -- Create date: <2-1-2021>
                    -- Description:	<Get Tenant Services>
                    -- =============================================
                    CREATE PROCEDURE [dbo].[TenantServicesGet] 
	                    @TenantId	int
                    AS
                    BEGIN

                    select
                    @TenantId as TenantId
                    ,InfoSeedServices.Id as ServiceId
                    ,InfoSeedServices.ServiceName
                    ,InfoSeedServices.ServiceFees as Fees
                    ,cast(0 as bit) as IsSelected
                    ,getdate() as TenantServiceCreationTime into #AllServices from InfoSeedServices 


                    update  #AllServices
                    set IsSelected = cast(1 as bit)
                    ,TenantServiceCreationTime = TS.CreationTime
                    , Fees = ISNULL(TS.ServiceFees,S.Fees)

                    from
                    #AllServices as S
                    inner join TenantServices as TS 
                    on S.ServiceId = TS.InfoSeedServiceId and ts.TenantId = @TenantId

                    select * from #AllServices

                    drop table #AllServices

                    --SELECT        TenantServices.TenantId,
                    --InfoSeedServices.Id AS ServiceId,
                    --InfoSeedServices.ServiceName, 
                    --ISNULL(TenantServices.ServiceFees, 
                    --InfoSeedServices.ServiceFees) AS Fees, 
                    --TenantServices.CreationTime as TenantServiceCreationTime,
                    --case when TenantServices.Id is null then cast(0 as bit) else cast(1 as bit) end as IsSelected
                    --FROM            TenantServices RIGHT OUTER JOIN
                    --                         InfoSeedServices ON TenantServices.InfoSeedServiceId = InfoSeedServices.Id
                    --WHERE        (TenantServices.TenantId = @TenantId) OR
                    --                         (TenantServices.Id IS NULL)
                    END
                    ";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
