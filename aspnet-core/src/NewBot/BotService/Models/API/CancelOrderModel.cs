namespace BotService.Models.API
{
    public class CancelOrderModel
    {
        public bool IsTrueOrder { get; set; }
        public string TextCancelOrder { get; set; }
        public bool CancelOrder { get; set; }

        public bool WrongOrder { get; set; }
    }
}
