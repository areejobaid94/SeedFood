using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.BotFlow.Dtos
{
    public class Cacatalog
    {// Product DTO
        public string CatalogId { get; set; }
        public string CatalogName { get; set; }
        public string sectionTitle { get; set; }

        public List<ProductDto> Products { get; set; }
    }
    public class ProductDto
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
        public string ImageUrl { get; set; }
    }
}
