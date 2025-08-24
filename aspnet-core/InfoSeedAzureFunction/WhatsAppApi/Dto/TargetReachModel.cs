using System;

namespace InfoSeedAzureFunction.WhatsAppApi.Dto
{
    public class TargetReachModel
    {
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public int LanguageId { get; set; }
        public string Message { get; set; }
        public string TemplateName { get; set; }
        public Guid GuidId { get; set; }
        public string UserId { get; set; }
        public int TenantId { get; set; }
        public string ResultJson { get; set; }
        public string MessageId { get; set; }
        public bool IsSent { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsRead { get; set; }
        public int ContactId { get; set; }
        public bool IsFailed { get; set; }
        public long TemplateId { get; set; }
        public bool IsFreeMessage { get; set; }
        public decimal MessageRate { get; set; }
        public Guid SentMessageTemplateId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
