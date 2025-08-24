using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.LiveChat.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.LiveChat.Exporting
{
    public interface ILiveChatExcelExporter
    {
        FileDto ExportToFile(List<CustomerLiveChatModel>  models);
    }
}
