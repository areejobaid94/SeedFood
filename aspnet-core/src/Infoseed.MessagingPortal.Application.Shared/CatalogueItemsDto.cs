using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infoseed.MessagingPortal
{
    public class CatalogueItemsDto
    {
        public List<ProductItem> Data { get; set; }
        public FbPaging Paging { get; set; }
    }
    public class ProductItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Retailer_Id { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
        public string Availability { get; set; }
        public string Image_Url { get; set; }
        public string Url { get; set; }
    }

    public class FbPaging
    {
        [JsonPropertyName("next")]
        public string Next { get; set; }
    }

}
