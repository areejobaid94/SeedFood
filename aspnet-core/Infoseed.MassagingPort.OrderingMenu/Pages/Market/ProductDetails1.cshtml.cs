using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Market
{
    public class ProductDetails2Model : PageModel
    {
        private readonly ILogger<ProductDetailsModel> _logger;
        [BindProperty]
        public Product Product {
            get; set;
        }

        public int Id { get; set; }
        public ProductDetails2Model(ILogger<ProductDetailsModel> logger)
        {
            _logger = logger;
        }


        public void OnGet(int? id)
        {
            this.Id = Id;
          
        }
    }
}