using System.Collections.Generic;

namespace Infoseed.MessagingPortal
{
    public class CatalogueDto
    {
        public List<CatalogueData> Data { get; set; }
    }

    public class CatalogueData
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
