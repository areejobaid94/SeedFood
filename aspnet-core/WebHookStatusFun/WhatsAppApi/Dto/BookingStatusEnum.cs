namespace WebHookStatusFun
{
    public enum BookingStatusEnum
    {
        Pending = 1,
        Confirmed = 2,
        Booked = 3,
        Canceled = 4,
        Deleted = 5,

    }

    public enum BookingTemplateNameEnum
    {
        booking_template_19 = 1,
        reminder_booking_19 = 2,

        booking_template_ar_19 = 3,
        reminder_booking_ar_19 = 4,

    }
    public enum BookingTemplateCaptionEnum
    {
        Confirmation = 1253,
        Reminder = 1254,
        Delete = 1256

    }
    public enum BookingTypeEnum
    {
        Manual = 1,
        WhatsApp = 2,
    }
    public enum BookingLanguage
    {
        Arabic = 1,
        English = 2,
    }

}
