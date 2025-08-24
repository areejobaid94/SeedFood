using System;

namespace InfoSeedAzureFunction.WhatsAppApi.Dto
{
    public class BookingModel
    {
        public long Id { get; set; }
        public int BookingNumber { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDateTime { get; set; }
        public string BookingDateTimeString { get; set; }
        public string BookingDate { get; set; }
        public string BookingTime { get; set; }
        public string ContactBookingTime { get; set; }
        public BookingStatusEnum BookingStatus { get; set; }
        public BookingTypeEnum BookingType { get; set; }


        public int StatusId
        {
            get { return (int)BookingStatus; }
            set { BookingStatus = (BookingStatusEnum)value; }
        }
        public int BookingTypeId
        {
            get { return (int)BookingType; }
            set { BookingType = (BookingTypeEnum)value; }
        }
        public long? AreaId { get; set; }
        public string AreaName { get; set; }
        public int TenantId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ContactId { get; set; }
        public long TemplateId { get; set; }
        public int LanguageId { get; set; }
        public string CustomerId { get; set; }
    }
}
