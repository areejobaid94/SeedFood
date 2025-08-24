namespace BotService.Models.API
{
    public class CreateOrEditItemAndAdditionsCategory
    {
        public int AdditionsAndItemId { get; set; }
        public long Id { get; set; }
        public int? TenantId { get; set; }

        public int ItemId { get; set; }

        public int SpecificationId { get; set; }
        public int AdditionsCategorysId { get; set; }

        public int MenuType { get; set; }
    }
}
