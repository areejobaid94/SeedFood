using Infoseed.MessagingPortal.WhatsApp.Dto;
using System;

namespace Infoseed.MessagingPortal.Web.Models.TeamInboxModels
{
    public class TeamInboxDto
    {
        public long templateId { get; set; }
        public string contactName { get; set; }
        public string phoneNumber { get; set; }
        public int campaignStatus { get; set; }
        public string language { get; set; }
        public string sendTime { get; set; }
        public bool isExternal { get; set; }
        public int CustomerOPT { get; set; }
        public TemplateVariablles templateVariables { get; set; }
        public HeaderVariablesTemplate headerVariabllesTemplate { get; set; }
        public FirstButtonURLVariabllesTemplate firstButtonURLVariabllesTemplate { get; set; }
        public SecondButtonURLVariabllesTemplate secondButtonURLVariabllesTemplate { get; set; }
        public CarouselVariabllesTemplate CarouselTemplate { get; set; }
        public ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate{ get; set; }
    }
    //public class TemplateVariables
    //{
    //    public string VarOne { get; set; }
    //    public string VarTwo { get; set; }
    //    public string VarThree { get; set; }
    //    public string VarFour { get; set; }
    //    public string VarFive { get; set; }
    //}
}
