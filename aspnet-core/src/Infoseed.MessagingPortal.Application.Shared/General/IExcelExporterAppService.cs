using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.General.Dto;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.General
{
    public interface IExcelExporterAppService 
    {
        FileDto ExportTenantsToFile(List<TenantsToExcelDto> model);
        FileDto ExportContactCampaignToFile(List<ContactCampaignDto> model);
        FileDto ExportItemsToExcel(List<ItemDto> items);
        FileDto ExportLiveChatToExcel(List<CustomerLiveChatModel> liveChat);
        FileDto ExportTenantsFileHost(List<ExportToExcelHost> model);
    }
}
