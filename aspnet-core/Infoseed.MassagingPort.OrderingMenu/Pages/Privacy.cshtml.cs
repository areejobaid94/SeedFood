using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.MenuCategories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Infoseed.MassagingPort.OrderingMenu.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private IMenuCategoriesAppService _IMenuCategoriesAppService;

        public PrivacyModel(ILogger<PrivacyModel> logger,
            IMenuCategoriesAppService iMenuCategoriesAppService

            )



        {
            _logger = logger;
            _IMenuCategoriesAppService = iMenuCategoriesAppService;
        }

        public void OnGet()
        {
            var result = _IMenuCategoriesAppService.GetCategoryWithItem(46, 27);

        }
    }
}