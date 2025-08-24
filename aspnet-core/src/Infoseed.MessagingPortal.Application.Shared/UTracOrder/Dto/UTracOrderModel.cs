using System.Collections.Generic;

namespace Infoseed.MessagingPortal.UTracOrder.Dto
{
    public class UTracOrderModel
    {
        public string _id { get; set; }
        public string ordernumber { get; set; }
        public string status { get; set; }
        public string integrator_number { get; set; }
        public string _created_at { get; set; }
    }
    public class UTracSearchEntity
    {
        public UTracSearchModel SearchModel { get; set; }
        public UTracPaginationModel PaginationModel { get; set; }
    }
    public class UTracSearchModel
    {
        public string integrator_id { get; set; }
        public string integrator_number { get; set; }
        public string status { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
    }

    public class UTracPaginationModel
    {
        public int start { get; set; } = 1;
        public int length { get; set; }

    }

    public class UTracOrderResponseModel
    {
        public string status { get; set; }
        public UTracOrderResponseDataModel data { get; set; }
    }
    public class UTracOrderResponseDataModel
    {
        public int total_count { get; set; }
        public List<UTracOrderModel> orders { get; set; }
        public string message { get; set; }
    }
    















    public enum UTracOrderStatusEunm
    {
        Pending = 0,
        Done = 1,
        Cancel = 2
    }

    public class SubmitUTracOrder
    {
        public string BuildingNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }

        public int TenantID { get; set; }
        public int ContactId { get; set; }


    }
}
