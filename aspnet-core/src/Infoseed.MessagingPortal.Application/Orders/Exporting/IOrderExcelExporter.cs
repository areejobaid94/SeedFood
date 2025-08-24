using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders.Exporting
{
    public interface IOrderExcelExporter
    {
        FileDto ExportToFile(List<GetOrderForViewDto> orders);
    }
}