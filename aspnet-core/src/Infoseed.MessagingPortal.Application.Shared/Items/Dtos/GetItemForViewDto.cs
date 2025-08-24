namespace Infoseed.MessagingPortal.Items.Dtos
{
    public class GetItemForViewDto
    {
		public ItemDto Item { get; set; }

        public string MenuName{ get; set; }
        public string MenuNameEnglish { get; set; }
        public string CategoryName{ get; set; }
        public string CategoryNameEnglish { get; set; }
    }
}