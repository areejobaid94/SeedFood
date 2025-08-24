namespace Infoseed.MessagingPortal.Areas.Dtos
{
    public class GetAreaForViewDto
    {
		public AreaDto Area { get; set; }


        public bool IsRestaurantsTypeAll { get; set; }
        public  bool IsAvailableBranch { get; set; }
        public string RestaurantsName { get; set; }
        public string UserName { get; set; }
        public string Surname { get; set; }
        
    }
}