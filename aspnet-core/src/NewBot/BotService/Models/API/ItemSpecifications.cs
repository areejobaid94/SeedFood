namespace BotService.Models.API
{
    public class ItemSpecifications
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }

        public long ItemId { get; set; }
        public int SpecificationId { get; set; }

        public bool IsRequired { get; set; }

        public int MaxSelectNumber { get; set; }
    }
}
