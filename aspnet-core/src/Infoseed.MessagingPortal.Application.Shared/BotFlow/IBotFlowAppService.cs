using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.BotFlow.Dtos;
using Infoseed.MessagingPortal.BranchAreas.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotFlow
{
    public interface IBotFlowAppService : IApplicationService
    {
        Task<PagedResultDto<GetBotModelFlowForViewDto>> GetAllBotFlows(int TenantId, int? PageNumber = 0, int? PageSize = 0);

        GetBotModelFlowForViewDto GetByIdBotFlows(long Id);

        long UpdateBotFlowsIsPublished(long Id, long modifiedUserId, string modifiedUserName, bool isPublished, string BotChannel = "whatsapp");

        long UpdateBotFlowsJsonModel(long Id , GetBotModelFlowForViewDto mdeol);

        long CreateBotFlows(GetBotModelFlowForViewDto mdeol);

        long DeleteBotFlows(long Id);

        long BotParameterCreate(BotParameterModel mdeol);

        long BotParameterDeleteById(long Id);

        Task<PagedResultDto<BotParameterModel>> BotParameterGetAll(int TenantId);
        void UpdateBotReStart(int TenantId);
        Task<long> GetBotFlowsById(long id);
        Task<List<IList<object>>> GetSheetValues(string spreadsheetId, string sheetName, string lookUpColumn, string filterValue, int tenantId);
        Task<string> InsertRow(InsertSheetRowDto rowDto);
        Task<List<string>> GetWorkSheets(string spreadsheetId, int tenantId);
        Task<List<string>> GetLookupHeaders(string spreadsheetId, string sheetName, int tenantId);
        Task<GetSpreadSheetsDto> GetSpreadSheets(int tenantId);



    }
}
