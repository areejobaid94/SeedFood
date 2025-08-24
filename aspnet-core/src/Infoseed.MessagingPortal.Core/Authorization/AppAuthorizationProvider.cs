using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Infoseed.MessagingPortal.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));
            pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_TeamInbox, L("TeamInbox"), multiTenancySides: MultiTenancySides.Tenant);



            var contacts = pages.CreateChildPermission(AppPermissions.Pages_Contacts, L("Contacts"), multiTenancySides: MultiTenancySides.Tenant);
            contacts.CreateChildPermission(AppPermissions.Pages_Contacts_Create, L("CreateNewContact"));
            contacts.CreateChildPermission(AppPermissions.Pages_Contacts_Edit, L("EditContact"));
            contacts.CreateChildPermission(AppPermissions.Pages_Contacts_Delete, L("DeleteContact"));
            contacts.CreateChildPermission(AppPermissions.Pages_Contacts_ExternalContact, L("ExternalContact"));
            contacts.CreateChildPermission(AppPermissions.Pages_Contacts_Export_to_Excel, L("Export to Excel"));
            contacts.CreateChildPermission(AppPermissions.Pages_Contacts_Backup_All_Converstion, L("Backup All Converstion"));
            contacts.CreateChildPermission(AppPermissions.Pages_Contacts_ChangeBehaviour, L("ChangeContactBehaviour"));


            pages.CreateChildPermission(AppPermissions.Pages_Menus, L("Menus"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_Evaluation, L("Evaluation"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_Branches, L("Branches"), multiTenancySides: MultiTenancySides.Tenant);

            pages.CreateChildPermission(AppPermissions.Pages_DeliveryOrders, L("DeliveryOrders"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_DeliveryLocation, L("DeliveryLocation"), multiTenancySides: MultiTenancySides.Tenant);

            pages.CreateChildPermission(AppPermissions.Pages_UpdateItem, L("UpdateItem"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_Booking, L("Booking"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_OrdersMaintenances, L("OrdersMaintenances"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_Orders, L("Orders"), multiTenancySides: MultiTenancySides.Tenant);

            pages.CreateChildPermission(AppPermissions.Pages_Location, L("Location"), multiTenancySides: MultiTenancySides.Tenant);




            var Channels = pages.CreateChildPermission(AppPermissions.Pages_Channels, L("Channels"));

            Channels.CreateChildPermission(AppPermissions.Pages_Facebook, L("Facebook"));
            Channels.CreateChildPermission(AppPermissions.Pages_Instagram, L("Instagram"));




            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));



            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Unlock, L("Unlock"));


            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));

            var billings = administration.CreateChildPermission(AppPermissions.Pages_Billings, L("Billings"));
            billings.CreateChildPermission(AppPermissions.Pages_Billings_Create, L("CreateNewBilling"));
            billings.CreateChildPermission(AppPermissions.Pages_Billings_Edit, L("EditBilling"));
            billings.CreateChildPermission(AppPermissions.Pages_Billings_Delete, L("DeleteBilling"));

            var templateMessages = administration.CreateChildPermission(AppPermissions.Pages_Administration_TemplateMessages, L("QuickAnswer"));
            templateMessages.CreateChildPermission(AppPermissions.Pages_Administration_TemplateMessages_Create, L("CreateNewQuickAnswer"));
            templateMessages.CreateChildPermission(AppPermissions.Pages_Administration_TemplateMessages_Edit, L("EditQuickAnswer"));
            templateMessages.CreateChildPermission(AppPermissions.Pages_Administration_TemplateMessages_Delete, L("DeleteQuickAnswer"));



            var Settings =administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
            Settings.CreateChildPermission(AppPermissions.Manage_Ticket_Setting, L("Manage_Ticket_Setting"));


            var close_Deals = pages.CreateChildPermission(AppPermissions.Pages_Close_Deals, L("Close_Deals"), multiTenancySides: MultiTenancySides.Host);
            close_Deals.CreateChildPermission(AppPermissions.Pages_Close_Deals_Create, L("CreateNewClose_Deal"), multiTenancySides: MultiTenancySides.Host);
            close_Deals.CreateChildPermission(AppPermissions.Pages_Close_Deals_Edit, L("EditClose_Deal"), multiTenancySides: MultiTenancySides.Host);
            close_Deals.CreateChildPermission(AppPermissions.Pages_Close_Deals_Delete, L("DeleteClose_Deal"), multiTenancySides: MultiTenancySides.Host);

            var closeDealStatuses = pages.CreateChildPermission(AppPermissions.Pages_CloseDealStatuses, L("CloseDealStatuses"), multiTenancySides: MultiTenancySides.Host);
            closeDealStatuses.CreateChildPermission(AppPermissions.Pages_CloseDealStatuses_Create, L("CreateNewCloseDealStatus"), multiTenancySides: MultiTenancySides.Host);
            closeDealStatuses.CreateChildPermission(AppPermissions.Pages_CloseDealStatuses_Edit, L("EditCloseDealStatus"), multiTenancySides: MultiTenancySides.Host);
            closeDealStatuses.CreateChildPermission(AppPermissions.Pages_CloseDealStatuses_Delete, L("DeleteCloseDealStatus"), multiTenancySides: MultiTenancySides.Host);

            //var deals = pages.CreateChildPermission(AppPermissions.Pages_Deals, L("Deals"));
            //deals.CreateChildPermission(AppPermissions.Pages_Deals_Create, L("CreateNewDeal"));
            //deals.CreateChildPermission(AppPermissions.Pages_Deals_Edit, L("EditDeal"));
            //deals.CreateChildPermission(AppPermissions.Pages_Deals_Delete, L("DeleteDeal"));

            //var dealTypes = pages.CreateChildPermission(AppPermissions.Pages_DealTypes, L("DealTypes"));
            //dealTypes.CreateChildPermission(AppPermissions.Pages_DealTypes_Create, L("CreateNewDealType"));
            //dealTypes.CreateChildPermission(AppPermissions.Pages_DealTypes_Edit, L("EditDealType"));
            //dealTypes.CreateChildPermission(AppPermissions.Pages_DealTypes_Delete, L("DeleteDealType"));

            var dealStatuses = pages.CreateChildPermission(AppPermissions.Pages_DealStatuses, L("DealStatuses"), multiTenancySides: MultiTenancySides.Host);
            dealStatuses.CreateChildPermission(AppPermissions.Pages_DealStatuses_Create, L("CreateNewDealStatus"), multiTenancySides: MultiTenancySides.Host);
            dealStatuses.CreateChildPermission(AppPermissions.Pages_DealStatuses_Edit, L("EditDealStatus"), multiTenancySides: MultiTenancySides.Host);
            dealStatuses.CreateChildPermission(AppPermissions.Pages_DealStatuses_Delete, L("DeleteDealStatus"), multiTenancySides: MultiTenancySides.Host);

            var forcatses = pages.CreateChildPermission(AppPermissions.Pages_Forcatses, L("Forcatses"), multiTenancySides: MultiTenancySides.Host);
            forcatses.CreateChildPermission(AppPermissions.Pages_Forcatses_Create, L("CreateNewForcats"), multiTenancySides: MultiTenancySides.Host);
            forcatses.CreateChildPermission(AppPermissions.Pages_Forcatses_Edit, L("EditForcats"), multiTenancySides: MultiTenancySides.Host);
            forcatses.CreateChildPermission(AppPermissions.Pages_Forcatses_Delete, L("DeleteForcats"), multiTenancySides: MultiTenancySides.Host);

            var territories = pages.CreateChildPermission(AppPermissions.Pages_Territories, L("Territories"), multiTenancySides: MultiTenancySides.Host);
            territories.CreateChildPermission(AppPermissions.Pages_Territories_Create, L("CreateNewTerritory"), multiTenancySides: MultiTenancySides.Host);
            territories.CreateChildPermission(AppPermissions.Pages_Territories_Edit, L("EditTerritory"), multiTenancySides: MultiTenancySides.Host);
            territories.CreateChildPermission(AppPermissions.Pages_Territories_Delete, L("DeleteTerritory"), multiTenancySides: MultiTenancySides.Host);





            var receiptDetails = pages.CreateChildPermission(AppPermissions.Pages_ReceiptDetails, L("ReceiptDetails"), multiTenancySides: MultiTenancySides.Host);
            receiptDetails.CreateChildPermission(AppPermissions.Pages_ReceiptDetails_Create, L("CreateNewReceiptDetail"), multiTenancySides: MultiTenancySides.Host);
            receiptDetails.CreateChildPermission(AppPermissions.Pages_ReceiptDetails_Edit, L("EditReceiptDetail"), multiTenancySides: MultiTenancySides.Host);
            receiptDetails.CreateChildPermission(AppPermissions.Pages_ReceiptDetails_Delete, L("DeleteReceiptDetail"), multiTenancySides: MultiTenancySides.Host);

            var templateMessagePurposes = pages.CreateChildPermission(AppPermissions.Pages_TemplateMessagePurposes, L("TemplateMessagePurposes"), multiTenancySides: MultiTenancySides.Host);
            templateMessagePurposes.CreateChildPermission(AppPermissions.Pages_TemplateMessagePurposes_Create, L("CreateNewTemplateMessagePurpose"), multiTenancySides: MultiTenancySides.Host);
            templateMessagePurposes.CreateChildPermission(AppPermissions.Pages_TemplateMessagePurposes_Edit, L("EditTemplateMessagePurpose"), multiTenancySides: MultiTenancySides.Host);
            templateMessagePurposes.CreateChildPermission(AppPermissions.Pages_TemplateMessagePurposes_Delete, L("DeleteTemplateMessagePurpose"), multiTenancySides: MultiTenancySides.Host);

            ////var orderReceipts = pages.CreateChildPermission(AppPermissions.Pages_OrderReceipts, L("OrderReceipts"));
            ////orderReceipts.CreateChildPermission(AppPermissions.Pages_OrderReceipts_Create, L("CreateNewOrderReceipt"));
            ////orderReceipts.CreateChildPermission(AppPermissions.Pages_OrderReceipts_Edit, L("EditOrderReceipt"));
            ////orderReceipts.CreateChildPermission(AppPermissions.Pages_OrderReceipts_Delete, L("DeleteOrderReceipt"));

            ////var orderLineAdditionalIngredients = pages.CreateChildPermission(AppPermissions.Pages_OrderLineAdditionalIngredients, L("OrderLineAdditionalIngredients"));
            ////orderLineAdditionalIngredients.CreateChildPermission(AppPermissions.Pages_OrderLineAdditionalIngredients_Create, L("CreateNewOrderLineAdditionalIngredient"));
            ////orderLineAdditionalIngredients.CreateChildPermission(AppPermissions.Pages_OrderLineAdditionalIngredients_Edit, L("EditOrderLineAdditionalIngredient"));
            ////orderLineAdditionalIngredients.CreateChildPermission(AppPermissions.Pages_OrderLineAdditionalIngredients_Delete, L("DeleteOrderLineAdditionalIngredient"));

            ////var orderDetails = pages.CreateChildPermission(AppPermissions.Pages_OrderDetails, L("OrderDetails"));
            ////orderDetails.CreateChildPermission(AppPermissions.Pages_OrderDetails_Create, L("CreateNewOrderDetail"));
            ////orderDetails.CreateChildPermission(AppPermissions.Pages_OrderDetails_Edit, L("EditOrderDetail"));
            ////orderDetails.CreateChildPermission(AppPermissions.Pages_OrderDetails_Delete, L("DeleteOrderDetail"));

            //var orders = pages.CreateChildPermission(AppPermissions.Pages_Orders, L("Orders"));
            //orders.CreateChildPermission(AppPermissions.Pages_Orders_Create, L("CreateNewOrder"));
            //orders.CreateChildPermission(AppPermissions.Pages_Orders_Edit, L("EditOrder"));
            //orders.CreateChildPermission(AppPermissions.Pages_Orders_Delete, L("DeleteOrder"));

            ////var customers = pages.CreateChildPermission(AppPermissions.Pages_Customers, L("Customers"));
            ////customers.CreateChildPermission(AppPermissions.Pages_Customers_Create, L("CreateNewCustomer"));
            ////customers.CreateChildPermission(AppPermissions.Pages_Customers_Edit, L("EditCustomer"));
            ////customers.CreateChildPermission(AppPermissions.Pages_Customers_Delete, L("DeleteCustomer"));

            ////var orderStatuses = pages.CreateChildPermission(AppPermissions.Pages_OrderStatuses, L("OrderStatuses"));
            ////orderStatuses.CreateChildPermission(AppPermissions.Pages_OrderStatuses_Create, L("CreateNewOrderStatus"));
            ////orderStatuses.CreateChildPermission(AppPermissions.Pages_OrderStatuses_Edit, L("EditOrderStatus"));
            ////orderStatuses.CreateChildPermission(AppPermissions.Pages_OrderStatuses_Delete, L("DeleteOrderStatus"));

            ////var genders = pages.CreateChildPermission(AppPermissions.Pages_Genders, L("Genders"));
            ////genders.CreateChildPermission(AppPermissions.Pages_Genders_Create, L("CreateNewGender"));
            ////genders.CreateChildPermission(AppPermissions.Pages_Genders_Edit, L("EditGender"));
            ////genders.CreateChildPermission(AppPermissions.Pages_Genders_Delete, L("DeleteGender"));

            ////var cities = pages.CreateChildPermission(AppPermissions.Pages_Cities, L("Cities"));
            ////cities.CreateChildPermission(AppPermissions.Pages_Cities_Create, L("CreateNewCity"));
            ////cities.CreateChildPermission(AppPermissions.Pages_Cities_Edit, L("EditCity"));
            ////cities.CreateChildPermission(AppPermissions.Pages_Cities_Delete, L("DeleteCity"));

            ////var branches = pages.CreateChildPermission(AppPermissions.Pages_Branches, L("Branches"));
            ////branches.CreateChildPermission(AppPermissions.Pages_Branches_Create, L("CreateNewBranch"));
            ////branches.CreateChildPermission(AppPermissions.Pages_Branches_Edit, L("EditBranch"));
            ////branches.CreateChildPermission(AppPermissions.Pages_Branches_Delete, L("DeleteBranch"));

            ////var deliveryChanges = pages.CreateChildPermission(AppPermissions.Pages_DeliveryChanges, L("DeliveryChanges"));
            ////deliveryChanges.CreateChildPermission(AppPermissions.Pages_DeliveryChanges_Create, L("CreateNewDeliveryChange"));
            ////deliveryChanges.CreateChildPermission(AppPermissions.Pages_DeliveryChanges_Edit, L("EditDeliveryChange"));
            ////deliveryChanges.CreateChildPermission(AppPermissions.Pages_DeliveryChanges_Delete, L("DeleteDeliveryChange"));

            ////var branchAreas = pages.CreateChildPermission(AppPermissions.Pages_BranchAreas, L("BranchAreas"));
            ////branchAreas.CreateChildPermission(AppPermissions.Pages_BranchAreas_Create, L("CreateNewBranchArea"));
            ////branchAreas.CreateChildPermission(AppPermissions.Pages_BranchAreas_Edit, L("EditBranchArea"));
            ////branchAreas.CreateChildPermission(AppPermissions.Pages_BranchAreas_Delete, L("DeleteBranchArea"));

            ////var areas = pages.CreateChildPermission(AppPermissions.Pages_Areas, L("Areas"));
            ////areas.CreateChildPermission(AppPermissions.Pages_Areas_Create, L("CreateNewArea"));
            ////areas.CreateChildPermission(AppPermissions.Pages_Areas_Edit, L("EditArea"));
            ////areas.CreateChildPermission(AppPermissions.Pages_Areas_Delete, L("DeleteArea"));

            ////var menuDetails = pages.CreateChildPermission(AppPermissions.Pages_MenuDetails, L("MenuDetails"));
            ////menuDetails.CreateChildPermission(AppPermissions.Pages_MenuDetails_Create, L("CreateNewMenuDetail"));
            ////menuDetails.CreateChildPermission(AppPermissions.Pages_MenuDetails_Edit, L("EditMenuDetail"));
            ////menuDetails.CreateChildPermission(AppPermissions.Pages_MenuDetails_Delete, L("DeleteMenuDetail"));

            ////var items = pages.CreateChildPermission(AppPermissions.Pages_Items, L("Items"));
            ////items.CreateChildPermission(AppPermissions.Pages_Items_Create, L("CreateNewItem"));
            ////items.CreateChildPermission(AppPermissions.Pages_Items_Edit, L("EditItem"));
            ////items.CreateChildPermission(AppPermissions.Pages_Items_Delete, L("DeleteItem"));

            ////var menus = pages.CreateChildPermission(AppPermissions.Pages_Menus, L("Menus"));
            ////menus.CreateChildPermission(AppPermissions.Pages_Menus_Create, L("CreateNewMenu"));
            ////menus.CreateChildPermission(AppPermissions.Pages_Menus_Edit, L("EditMenu"));
            ////menus.CreateChildPermission(AppPermissions.Pages_Menus_Delete, L("DeleteMenu"));

            ////var menuCategories = pages.CreateChildPermission(AppPermissions.Pages_MenuCategories, L("MenuCategories"));
            ////menuCategories.CreateChildPermission(AppPermissions.Pages_MenuCategories_Create, L("CreateNewMenuCategory"));
            ////menuCategories.CreateChildPermission(AppPermissions.Pages_MenuCategories_Edit, L("EditMenuCategory"));
            ////menuCategories.CreateChildPermission(AppPermissions.Pages_MenuCategories_Delete, L("DeleteMenuCategory"));

            var menuItemStatuses = pages.CreateChildPermission(AppPermissions.Pages_MenuItemStatuses, L("MenuItemStatuses"), multiTenancySides: MultiTenancySides.Host);
            menuItemStatuses.CreateChildPermission(AppPermissions.Pages_MenuItemStatuses_Create, L("CreateNewMenuItemStatus"), multiTenancySides: MultiTenancySides.Host);
            menuItemStatuses.CreateChildPermission(AppPermissions.Pages_MenuItemStatuses_Edit, L("EditMenuItemStatus"), multiTenancySides: MultiTenancySides.Host);
            menuItemStatuses.CreateChildPermission(AppPermissions.Pages_MenuItemStatuses_Delete, L("DeleteMenuItemStatus"), multiTenancySides: MultiTenancySides.Host);

            //pages.CreateChildPermission(AppPermissions.Pages_DemoUiComponents, L("DemoUiComponents"));

            //var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

            var chatStatuses = administration.CreateChildPermission(AppPermissions.Pages_Administration_ChatStatuses, L("ChatStatuses"), multiTenancySides: MultiTenancySides.Host);
            chatStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ChatStatuses_Create, L("CreateNewChatStatuse"), multiTenancySides: MultiTenancySides.Host);
            chatStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ChatStatuses_Edit, L("EditChatStatuse"), multiTenancySides: MultiTenancySides.Host);
            chatStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ChatStatuses_Delete, L("DeleteChatStatuse"), multiTenancySides: MultiTenancySides.Host);

            var contactStatuses = administration.CreateChildPermission(AppPermissions.Pages_Administration_ContactStatuses, L("ContactStatuses"), multiTenancySides: MultiTenancySides.Host);
            contactStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ContactStatuses_Create, L("CreateNewContactStatuse"), multiTenancySides: MultiTenancySides.Host);
            contactStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ContactStatuses_Edit, L("EditContactStatuse"), multiTenancySides: MultiTenancySides.Host);
            contactStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ContactStatuses_Delete, L("DeleteContactStatuse"), multiTenancySides: MultiTenancySides.Host);

            //var receipts = administration.CreateChildPermission(AppPermissions.Pages_Administration_Receipts, L("Receipts"));
            //receipts.CreateChildPermission(AppPermissions.Pages_Administration_Receipts_Create, L("CreateNewReceipt"));
            //receipts.CreateChildPermission(AppPermissions.Pages_Administration_Receipts_Edit, L("EditReceipt"));
            //receipts.CreateChildPermission(AppPermissions.Pages_Administration_Receipts_Delete, L("DeleteReceipt"));

            var banks = administration.CreateChildPermission(AppPermissions.Pages_Administration_Banks, L("Banks"), multiTenancySides: MultiTenancySides.Host);
            banks.CreateChildPermission(AppPermissions.Pages_Administration_Banks_Create, L("CreateNewBank"), multiTenancySides: MultiTenancySides.Host);
            banks.CreateChildPermission(AppPermissions.Pages_Administration_Banks_Edit, L("EditBank"), multiTenancySides: MultiTenancySides.Host);
            banks.CreateChildPermission(AppPermissions.Pages_Administration_Banks_Delete, L("DeleteBank"), multiTenancySides: MultiTenancySides.Host);

            var paymentMethods = administration.CreateChildPermission(AppPermissions.Pages_Administration_PaymentMethods, L("PaymentMethods"), multiTenancySides: MultiTenancySides.Host);
            paymentMethods.CreateChildPermission(AppPermissions.Pages_Administration_PaymentMethods_Create, L("CreateNewPaymentMethod"), multiTenancySides: MultiTenancySides.Host);
            paymentMethods.CreateChildPermission(AppPermissions.Pages_Administration_PaymentMethods_Edit, L("EditPaymentMethod"), multiTenancySides: MultiTenancySides.Host);
            paymentMethods.CreateChildPermission(AppPermissions.Pages_Administration_PaymentMethods_Delete, L("DeletePaymentMethod"), multiTenancySides: MultiTenancySides.Host);

            var tenantServices = administration.CreateChildPermission(AppPermissions.Pages_Administration_TenantServices, L("TenantServices"), multiTenancySides: MultiTenancySides.Host);
            tenantServices.CreateChildPermission(AppPermissions.Pages_Administration_TenantServices_Create, L("CreateNewTenantService"));
            tenantServices.CreateChildPermission(AppPermissions.Pages_Administration_TenantServices_Edit, L("EditTenantService"));
            tenantServices.CreateChildPermission(AppPermissions.Pages_Administration_TenantServices_Delete, L("DeleteTenantService"));

            var accountBillings = administration.CreateChildPermission(AppPermissions.Pages_Administration_AccountBillings, L("AccountBillings"), multiTenancySides: MultiTenancySides.Host);
            accountBillings.CreateChildPermission(AppPermissions.Pages_Administration_AccountBillings_Create, L("CreateNewAccountBilling"), multiTenancySides: MultiTenancySides.Host);
            accountBillings.CreateChildPermission(AppPermissions.Pages_Administration_AccountBillings_Edit, L("EditAccountBilling"), multiTenancySides: MultiTenancySides.Host);
            accountBillings.CreateChildPermission(AppPermissions.Pages_Administration_AccountBillings_Delete, L("DeleteAccountBilling"), multiTenancySides: MultiTenancySides.Host);

            var tenantAccountBillings = administration.CreateChildPermission(AppPermissions.Pages_Administration_Hos_AccountBillings, L("AccountBillings"), multiTenancySides: MultiTenancySides.Host);

            var currencies = administration.CreateChildPermission(AppPermissions.Pages_Administration_Currencies, L("Currencies"), multiTenancySides: MultiTenancySides.Host);
            currencies.CreateChildPermission(AppPermissions.Pages_Administration_Currencies_Create, L("CreateNewCurrency"), multiTenancySides: MultiTenancySides.Host);
            currencies.CreateChildPermission(AppPermissions.Pages_Administration_Currencies_Edit, L("EditCurrency"), multiTenancySides: MultiTenancySides.Host);
            currencies.CreateChildPermission(AppPermissions.Pages_Administration_Currencies_Delete, L("DeleteCurrency"), multiTenancySides: MultiTenancySides.Host);

            var infoSeedServices = administration.CreateChildPermission(AppPermissions.Pages_Administration_InfoSeedServices, L("InfoSeedServices"), multiTenancySides: MultiTenancySides.Host);
            infoSeedServices.CreateChildPermission(AppPermissions.Pages_Administration_InfoSeedServices_Create, L("CreateNewInfoSeedService"), multiTenancySides: MultiTenancySides.Host);
            infoSeedServices.CreateChildPermission(AppPermissions.Pages_Administration_InfoSeedServices_Edit, L("EditInfoSeedService"), multiTenancySides: MultiTenancySides.Host);
            infoSeedServices.CreateChildPermission(AppPermissions.Pages_Administration_InfoSeedServices_Delete, L("DeleteInfoSeedService"), multiTenancySides: MultiTenancySides.Host);

            var serviceFrquencies = administration.CreateChildPermission(AppPermissions.Pages_Administration_ServiceFrquencies, L("ServiceFrquencies"), multiTenancySides: MultiTenancySides.Host);
            serviceFrquencies.CreateChildPermission(AppPermissions.Pages_Administration_ServiceFrquencies_Create, L("CreateNewServiceFrquency"), multiTenancySides: MultiTenancySides.Host);
            serviceFrquencies.CreateChildPermission(AppPermissions.Pages_Administration_ServiceFrquencies_Edit, L("EditServiceFrquency"), multiTenancySides: MultiTenancySides.Host);
            serviceFrquencies.CreateChildPermission(AppPermissions.Pages_Administration_ServiceFrquencies_Delete, L("DeleteServiceFrquency"), multiTenancySides: MultiTenancySides.Host);

            var serviceStatuses = administration.CreateChildPermission(AppPermissions.Pages_Administration_ServiceStatuses, L("ServiceStatuses"), multiTenancySides: MultiTenancySides.Host);
            serviceStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ServiceStatuses_Create, L("CreateNewServiceStatus"), multiTenancySides: MultiTenancySides.Host);
            serviceStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ServiceStatuses_Edit, L("EditServiceStatus"), multiTenancySides: MultiTenancySides.Host);
            serviceStatuses.CreateChildPermission(AppPermissions.Pages_Administration_ServiceStatuses_Delete, L("DeleteServiceStatus"), multiTenancySides: MultiTenancySides.Host);

            var serviceTypes = administration.CreateChildPermission(AppPermissions.Pages_Administration_ServiceTypes, L("ServiceTypes"), multiTenancySides: MultiTenancySides.Host);
            serviceTypes.CreateChildPermission(AppPermissions.Pages_Administration_ServiceTypes_Create, L("CreateNewServiceType"), multiTenancySides: MultiTenancySides.Host);
            serviceTypes.CreateChildPermission(AppPermissions.Pages_Administration_ServiceTypes_Edit, L("EditServiceType"), multiTenancySides: MultiTenancySides.Host);
            serviceTypes.CreateChildPermission(AppPermissions.Pages_Administration_ServiceTypes_Delete, L("DeleteServiceType"), multiTenancySides: MultiTenancySides.Host);


            var Usage_details = administration.CreateChildPermission(AppPermissions.Pages_Administration_Usage_details, L("Usage details"));


            var languages = administration.CreateChildPermission(AppPermissions.Pages_Administration_Languages, L("Languages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Create, L("CreatingNewLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Edit, L("EditingLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Delete, L("DeletingLanguages"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_ChangeTexts, L("Keywords"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_AuditLogs, L("AuditLogs"), multiTenancySides: MultiTenancySides.Host);

            var organizationUnits = administration.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"), multiTenancySides: MultiTenancySides.Host);
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"), multiTenancySides: MultiTenancySides.Host);
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"), multiTenancySides: MultiTenancySides.Host);
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingRoles"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_UiCustomization, L("VisualSettings"), multiTenancySides: MultiTenancySides.Host);

            var webhooks = administration.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription, L("Webhooks"), multiTenancySides: MultiTenancySides.Host);
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Create, L("CreatingWebhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Edit, L("EditingWebhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_ChangeActivity, L("ChangingWebhookActivity"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Detail, L("DetailingSubscription"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_Webhook_ListSendAttempts, L("ListingSendAttempts"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_Webhook_ResendWebhook, L("ResendingWebhook"));

            var dynamicProperties = administration.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties, L("DynamicProperties"), multiTenancySides: MultiTenancySides.Host);
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Create, L("CreatingDynamicProperties"));
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Edit, L("EditingDynamicProperties"));
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Delete, L("DeletingDynamicProperties"));

            var dynamicPropertyValues = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue, L("DynamicPropertyValue"), multiTenancySides: MultiTenancySides.Host);
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Create, L("CreatingDynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Edit, L("EditingDynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Delete, L("DeletingDynamicPropertyValue"));

            var dynamicEntityProperties = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties, L("DynamicEntityProperties"), multiTenancySides: MultiTenancySides.Host);
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Create, L("CreatingDynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Edit, L("EditingDynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Delete, L("DeletingDynamicEntityProperties"));

            var dynamicEntityPropertyValues = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue, L("EntityDynamicPropertyValue"), multiTenancySides: MultiTenancySides.Host);
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Create, L("CreatingDynamicEntityPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Edit, L("EditingDynamicEntityPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Delete, L("DeletingDynamicEntityPropertyValue"));

            ////TENANT-SPECIFIC PERMISSIONS

            //pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);

            //administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
            //administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement, L("Subscription"), multiTenancySides: MultiTenancySides.Tenant);

            ////HOST-SPECIFIC PERMISSIONS

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_MoveTenantsToAnotherEdition, L("MoveTenantsToAnotherEdition"), multiTenancySides: MultiTenancySides.Host);

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Settings, L("SettingsTenant"), multiTenancySides: MultiTenancySides.Host);

            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);

            //pages.CreateChildPermission(AppPermissions.Pages_TeamInbox, L("TeamInbox"), multiTenancySides: MultiTenancySides.Tenant);



            //pages.CreateChildPermission(AppPermissions.Pages_Assets, L("Assets"), multiTenancySides: MultiTenancySides.Tenant);

            //   pages.CreateChildPermission(AppPermissions.Pages_LiveChat, L("Ticket"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_DeliveryCost, L("DeliveryCost"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_MessageTemplate, L("Template"), multiTenancySides: MultiTenancySides.Tenant);
            var campaign = pages.CreateChildPermission(AppPermissions.Pages_MessageCampaign, L("Campaign"), multiTenancySides: MultiTenancySides.Tenant);
            campaign.CreateChildPermission(AppPermissions.Pages_Campaign_UnsubscribedGroup, L("CreateUnsubiscribedGroup"));
            //pages.CreateChildPermission(AppPermissions.Pages_MessageConversation, L("MessageConversation"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_SendCampaign, L("SendCampaign"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_Offers, L("offers"), multiTenancySides: MultiTenancySides.Tenant);
            //pages.CreateChildPermission(AppPermissions.Pages_Department, L("Department"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_Botflow, L("Botflow"), multiTenancySides: MultiTenancySides.Tenant);

            //
            var ticket = pages.CreateChildPermission(AppPermissions.Pages_LiveChat, L("Ticket"), multiTenancySides: MultiTenancySides.Tenant);
            var chat = ticket.CreateChildPermission(AppPermissions.Pages_Ticket_Chat, L("Chat"));
            var openAndClose = ticket.CreateChildPermission(AppPermissions.Pages_Ticket_openAndClose, L("Open&Close"));
            var reopen = ticket.CreateChildPermission(AppPermissions.Pages_Ticket_reopen, L("Reopen"));
            var AssignToUser = ticket.CreateChildPermission(AppPermissions.Pages_Ticket_AssignToUser, L("AssignToUser"));
            var export_to_Excel = ticket.CreateChildPermission(AppPermissions.Pages_Ticket_Export_to_Excel, L("Export_to_Excel"));

            var Ticket_Close_All_Ticet = ticket.CreateChildPermission(AppPermissions.Pages_Ticket_Close_All_Ticet, L("Ticket_Close_All_Ticet"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, MessagingPortalConsts.LocalizationSourceName);
        }
    }
}