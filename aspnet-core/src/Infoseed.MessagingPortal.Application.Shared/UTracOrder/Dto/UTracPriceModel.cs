using System.Collections.Generic;

namespace Infoseed.MessagingPortal.UTracOrder.Dto
{
    public class UTracPriceModel
    {
        public string _id { get; set; }
        public string title { get; set; }
    }
    public class UTracPriceResponseModel
    {
        public string status { get; set; }
        public UTracPriceResponseDataModel data { get; set; }
    }

    public class UTracPriceResponseDataModel
    {
        public List<UTracPriceModel> prices { get; set; }
    }
}
