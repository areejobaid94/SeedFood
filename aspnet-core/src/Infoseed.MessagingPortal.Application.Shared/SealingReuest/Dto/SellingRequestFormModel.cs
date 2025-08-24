using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.SealingReuest.Dto
{
    public  class SellingRequestFormModel
    {
        public string Location { get; set; }
        public string ContentPlate { get; set; }
        //public string Price { get; set; }
        public string ContractName { get; set; }
        public string PhoneNumber { get; set; }
        public string PlaceResidence { get; set; }
        public string PaymentType { get; set; }
        public string BankDeposit { get; set; }
        public string InsideJordan { get; set; }
        public string OutsideJordan { get; set; }
    }
}
