using System.Globalization;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class OrderingMenuTenantModel
    {


        public string LogoImag { get; set; }
        public string BgImag { get; set; }

        public string Name { get; set; }

        public string NameEnglish { get; set; }
        public string CurrencyCode { get; set; }
        public string PhoneNumber { get; set; }



        public int TenantID { get; set; }


        public int ContactId { get; set; }

        public string UrlKey { get; set; }



        public int AreaId { get; set; }

    public int LanguageBot { get; set; }

    public string? Lang { get; set; }
        public bool IsApplyLoyalty { get; set; }
    public  OrderingMenuContactLoyaltyModel orderingMenuContactLoyaltyModel { get; set; }

        public OrderingMenuLoyaltyModel orderingMenuLoyaltyModel { get; set; }
        public  string DisplayName { get; set; }


    }




}
