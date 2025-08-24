namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppEnum
    {
        public enum WhatsContentTypeEnum
        {
            video,
            audio,
            document,
            text,
            image,
            file,
            location,
            interactive,
            template ,
            catalog ,
            order
        }
        public enum WhatsAppCampaignStatusEnum
        {
            Draft=0,      
            Sent=1,
            Scheduled = 2,
            Active =3,
            InActive =4,
        }
        public enum WhatsAppCampaignTypeEnum
        {
            Campaign = 1,
            FreeMessage = 2
       
        }
        public enum WhatsAppLanguageEnum
        {
            en = 1,
            ar = 2

        }
        public enum WhatsAppCategoryEnum
        {
            UTILITY = 1,
            MARKETING = 2,
            AUTHENTICATION = 3

        }
        public enum WhatsAppComponentTypeEnum
        {
            HEADER = 1,
            BODY = 2,
            FOOTER = 3,
            BUTTONS = 4

        }
        public enum WhatsAppHeaderFormatEnum
        {
            NONE = 1,
            TEXT = 2,
            DOCUMENT = 3,
            IMAGE = 4,
            VIDEO = 5

        }
        public enum WhatsAppButtonTypeEnum
        {
            NONE = 1,
            QUICK_REPLY = 2,
            PHONE_NUMBER = 3,
            URL = 4
        }
        public enum WhatsAppTemplateStatusEnum
        {
            PENDING = 1,
            APPROVED = 2,
            REJECTED = 3,
            PAUSED = 4,
            IN_APPEAL = 5,
            PENDING_DELETION = 6,
            DISABLED = 7,
            LIMIT_EXCEEDED = 8,
            DELETED = 9,
        }

        public enum WhatsAppConversationDirectionEnum
        {
            UNKNOWN,
            USER_INITIATED,
            BUSINESS_INITIATED
        }
        public enum WhatsAppConversationTypeEnum
        {
            UNKNOWN,
            REGULAR,
            FREE_TIER,
            FREE_ENTRY_POINT
        }
        public enum WhatsAppConversationCategoryEnum
        {
            UNKNOWN,
            MARKETING,
            UTILITY,
            AUTHENTICATION,
            SERVICE 
        }
    }   
}
