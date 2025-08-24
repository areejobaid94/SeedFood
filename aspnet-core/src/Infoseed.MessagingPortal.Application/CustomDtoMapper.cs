using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.DynamicEntityProperties;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using Abp.Webhooks;
using AutoMapper;
using Infoseed.MessagingPortal.AccountBillings;
using Infoseed.MessagingPortal.AccountBillings.Dtos;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Auditing.Dto;
using Infoseed.MessagingPortal.Authorization.Accounts.Dto;
using Infoseed.MessagingPortal.Authorization.Delegation;
using Infoseed.MessagingPortal.Authorization.Permissions.Dto;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.Authorization.Roles.Dto;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Authorization.Users.Delegation.Dto;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Authorization.Users.Importing.Dto;
using Infoseed.MessagingPortal.Authorization.Users.Profile.Dto;
using Infoseed.MessagingPortal.Banks;
using Infoseed.MessagingPortal.Banks.Dtos;
using Infoseed.MessagingPortal.Billings;
using Infoseed.MessagingPortal.Billings.Dtos;
using Infoseed.MessagingPortal.BranchAreas;
using Infoseed.MessagingPortal.BranchAreas.Dtos;
using Infoseed.MessagingPortal.Branches;
using Infoseed.MessagingPortal.Branches.Dtos;
using Infoseed.MessagingPortal.Chat;
using Infoseed.MessagingPortal.Chat.Dto;
using Infoseed.MessagingPortal.ChatStatuses;
using Infoseed.MessagingPortal.ChatStatuses.Dtos;
using Infoseed.MessagingPortal.Cities;
using Infoseed.MessagingPortal.Cities.Dtos;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.ContactStatuses;
using Infoseed.MessagingPortal.ContactStatuses.Dtos;
using Infoseed.MessagingPortal.Currencies;
using Infoseed.MessagingPortal.Currencies.Dtos;
using Infoseed.MessagingPortal.Customers;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.DeliveryChanges;
using Infoseed.MessagingPortal.DeliveryChanges.Dtos;
using Infoseed.MessagingPortal.DynamicEntityProperties.Dto;
using Infoseed.MessagingPortal.Editions;
using Infoseed.MessagingPortal.Editions.Dto;
using Infoseed.MessagingPortal.Forcasts;
using Infoseed.MessagingPortal.Forcasts.Dtos;
using Infoseed.MessagingPortal.Friendships;
using Infoseed.MessagingPortal.Friendships.Cache;
using Infoseed.MessagingPortal.Friendships.Dto;
using Infoseed.MessagingPortal.Genders;
using Infoseed.MessagingPortal.Genders.Dtos;
using Infoseed.MessagingPortal.InfoSeedServices;
using Infoseed.MessagingPortal.InfoSeedServices.Dtos;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Localization.Dto;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.MenuDetails;
using Infoseed.MessagingPortal.MenuDetails.Dtos;
using Infoseed.MessagingPortal.MenuItemStatuses;
using Infoseed.MessagingPortal.MenuItemStatuses.Dtos;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Menus.Dtos;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Payments;
using Infoseed.MessagingPortal.MultiTenancy.Payments.Dto;
using Infoseed.MessagingPortal.Notifications.Dto;
using Infoseed.MessagingPortal.OrderDetails;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.OrderLineAdditionalIngredients;
using Infoseed.MessagingPortal.OrderLineAdditionalIngredients.Dtos;
using Infoseed.MessagingPortal.OrderReceipts;
using Infoseed.MessagingPortal.OrderReceipts.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.OrderStatuses;
using Infoseed.MessagingPortal.OrderStatuses.Dtos;
using Infoseed.MessagingPortal.Organizations.Dto;
using Infoseed.MessagingPortal.PaymentMethods;
using Infoseed.MessagingPortal.PaymentMethods.Dtos;
using Infoseed.MessagingPortal.ReceiptDetails;
using Infoseed.MessagingPortal.ReceiptDetails.Dtos;
using Infoseed.MessagingPortal.Receipts;
using Infoseed.MessagingPortal.Receipts.Dtos;
using Infoseed.MessagingPortal.ServiceFrequencies;
using Infoseed.MessagingPortal.ServiceFrequencies.Dtos;
using Infoseed.MessagingPortal.ServiceStatuses;
using Infoseed.MessagingPortal.ServiceStatuses.Dtos;
using Infoseed.MessagingPortal.ServiceTypes;
using Infoseed.MessagingPortal.ServiceTypes.Dtos;
using Infoseed.MessagingPortal.Sessions.Dto;
using Infoseed.MessagingPortal.TemplateMessagePurposes;
using Infoseed.MessagingPortal.TemplateMessagePurposes.Dtos;
using Infoseed.MessagingPortal.TemplateMessages;
using Infoseed.MessagingPortal.TemplateMessages.Dtos;
using Infoseed.MessagingPortal.TenantServices;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using Infoseed.MessagingPortal.WebHooks.Dto;

namespace Infoseed.MessagingPortal
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditForcatsDto, Forcats>().ReverseMap();
            configuration.CreateMap<ForcatsDto, Forcats>().ReverseMap();
            configuration.CreateMap<CreateOrEditBillingDto, Billing>().ReverseMap();
            configuration.CreateMap<BillingDto, Billing>().ReverseMap();
            configuration.CreateMap<CreateOrEditContactDto, Contact>().ReverseMap();
            configuration.CreateMap<ContactDto, Contact>().ReverseMap();
            configuration.CreateMap<CreateOrEditChatStatuseDto, ChatStatuse>().ReverseMap();
            configuration.CreateMap<ChatStatuseDto, ChatStatuse>().ReverseMap();
            configuration.CreateMap<CreateOrEditContactStatuseDto, ContactStatuse>().ReverseMap();
            configuration.CreateMap<ContactStatuseDto, ContactStatuse>().ReverseMap();
            configuration.CreateMap<CreateOrEditReceiptDto, Receipt>().ReverseMap();
            configuration.CreateMap<ReceiptDto, Receipt>().ReverseMap();
            configuration.CreateMap<CreateOrEditReceiptDetailDto, ReceiptDetail>().ReverseMap();
            configuration.CreateMap<ReceiptDetailDto, ReceiptDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditBankDto, Bank>().ReverseMap();
            configuration.CreateMap<BankDto, Bank>().ReverseMap();
            configuration.CreateMap<CreateOrEditPaymentMethodDto, PaymentMethod>().ReverseMap();
            configuration.CreateMap<PaymentMethodDto, PaymentMethod>().ReverseMap();
            configuration.CreateMap<CreateOrEditTenantServiceDto, TenantService>().ReverseMap();
            configuration.CreateMap<TenantServiceDto, TenantService>().ReverseMap();
            configuration.CreateMap<CreateOrEditAccountBillingDto, AccountBilling>().ReverseMap();
            configuration.CreateMap<AccountBillingDto, AccountBilling>().ReverseMap();
            configuration.CreateMap<CreateOrEditCurrencyDto, Currency>().ReverseMap();
            configuration.CreateMap<CurrencyDto, Currency>().ReverseMap();
            configuration.CreateMap<CreateOrEditInfoSeedServiceDto, InfoSeedService>().ReverseMap();
            configuration.CreateMap<InfoSeedServiceDto, InfoSeedService>().ReverseMap();
            configuration.CreateMap<CreateOrEditServiceFrquencyDto, ServiceFrquency>().ReverseMap();
            configuration.CreateMap<ServiceFrquencyDto, ServiceFrquency>().ReverseMap();
            configuration.CreateMap<CreateOrEditServiceStatusDto, ServiceStatus>().ReverseMap();
            configuration.CreateMap<ServiceStatusDto, ServiceStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditServiceTypeDto, ServiceType>().ReverseMap();
            configuration.CreateMap<ServiceTypeDto, ServiceType>().ReverseMap();
            configuration.CreateMap<CreateOrEditTemplateMessageDto, TemplateMessage>().ReverseMap();
            configuration.CreateMap<TemplateMessageDto, TemplateMessage>().ReverseMap();
            configuration.CreateMap<CreateOrEditTemplateMessagePurposeDto, TemplateMessagePurpose>().ReverseMap();
            configuration.CreateMap<TemplateMessagePurposeDto, TemplateMessagePurpose>().ReverseMap();
            configuration.CreateMap<CreateOrEditOrderReceiptDto, OrderReceipt>().ReverseMap();
            configuration.CreateMap<OrderReceiptDto, OrderReceipt>().ReverseMap();
            configuration.CreateMap<CreateOrEditOrderLineAdditionalIngredientDto, OrderLineAdditionalIngredient>().ReverseMap();
            configuration.CreateMap<OrderLineAdditionalIngredientDto, OrderLineAdditionalIngredient>().ReverseMap();
            configuration.CreateMap<CreateOrEditOrderDetailDto, OrderDetail>().ReverseMap();
            configuration.CreateMap<OrderDetailDto, OrderDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditOrderDto, Order>().ReverseMap();
            configuration.CreateMap<OrderDto, Order>().ReverseMap();
            configuration.CreateMap<CreateOrEditCustomerDto, Customer>().ReverseMap();
            configuration.CreateMap<CustomerDto, Customer>().ReverseMap();
            configuration.CreateMap<CreateOrEditOrderStatusDto, OrderStatus>().ReverseMap();
            configuration.CreateMap<OrderStatusDto, OrderStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditGenderDto, Gender>().ReverseMap();
            configuration.CreateMap<GenderDto, Gender>().ReverseMap();
            configuration.CreateMap<CreateOrEditCityDto, City>().ReverseMap();
            configuration.CreateMap<CityDto, City>().ReverseMap();
            configuration.CreateMap<CreateOrEditBranchDto, Branch>().ReverseMap();
            configuration.CreateMap<BranchDto, Branch>().ReverseMap();
            configuration.CreateMap<CreateOrEditDeliveryChangeDto, DeliveryChange>().ReverseMap();
            configuration.CreateMap<DeliveryChangeDto, DeliveryChange>().ReverseMap();
            configuration.CreateMap<CreateOrEditBranchAreaDto, BranchArea>().ReverseMap();
            configuration.CreateMap<BranchAreaDto, BranchArea>().ReverseMap();
            configuration.CreateMap<CreateOrEditAreaDto, Area>().ReverseMap();
            configuration.CreateMap<AreaDto, Area>().ReverseMap();
            configuration.CreateMap<CreateOrEditMenuDetailDto, MenuDetail>().ReverseMap();
            configuration.CreateMap<MenuDetailDto, MenuDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditItemDto, Item>().ReverseMap();
            configuration.CreateMap<ItemDto, Item>().ReverseMap();
            configuration.CreateMap<CreateOrEditMenuDto, Menu>().ReverseMap();
            configuration.CreateMap<MenuDto, Menu>().ReverseMap();
            configuration.CreateMap<CreateOrEditMenuCategoryDto, ItemCategory>().ReverseMap();
            configuration.CreateMap<MenuCategoryDto, ItemCategory>().ReverseMap();
            configuration.CreateMap<CreateOrEditMenuItemStatusDto, MenuItemStatus>().ReverseMap();
            configuration.CreateMap<MenuItemStatusDto, MenuItemStatus>().ReverseMap();
            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Chat
            configuration.CreateMap<ChatMessage, ChatMessageDto>();
            configuration.CreateMap<ChatMessage, ChatMessageExportDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Role>().ReverseMap();
            configuration.CreateMap<Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Edition
            configuration.CreateMap<EditionEditDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<EditionCreateDto, SubscribableEdition>();
            configuration.CreateMap<EditionSelectDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<Edition, EditionInfoDto>().Include<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<SubscribableEdition, EditionListDto>();
            configuration.CreateMap<Edition, EditionEditDto>();
            configuration.CreateMap<Edition, SubscribableEdition>();
            configuration.CreateMap<Edition, EditionSelectDto>();

            //Payment
            configuration.CreateMap<SubscriptionPaymentDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPaymentListDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPayment, SubscriptionPaymentInfoDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Tenant
            configuration.CreateMap<Tenant, RecentTenant>();
            configuration.CreateMap<Tenant, TenantLoginInfoDto>();
            configuration.CreateMap<Tenant, TenantListDto>();
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, ChatUserDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();
            configuration.CreateMap<ImportUserDto, User>();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //Friendship
            configuration.CreateMap<Friendship, FriendDto>();
            configuration.CreateMap<FriendCacheItem, FriendDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();

            //Webhooks
            configuration.CreateMap<WebhookSubscription, GetAllSubscriptionsOutput>();
            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOutput>()
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.WebhookName,
                    options => options.MapFrom(l => l.WebhookEvent.WebhookName))
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.Data,
                    options => options.MapFrom(l => l.WebhookEvent.Data));

            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOfWebhookEventOutput>();

            configuration.CreateMap<DynamicProperty, DynamicPropertyDto>().ReverseMap();
            configuration.CreateMap<DynamicPropertyValue, DynamicPropertyValueDto>().ReverseMap();
            configuration.CreateMap<DynamicEntityProperty, DynamicEntityPropertyDto>()
                .ForMember(dto => dto.DynamicPropertyName,
                    options => options.MapFrom(entity => entity.DynamicProperty.PropertyName));
            configuration.CreateMap<DynamicEntityPropertyDto, DynamicEntityProperty>();

            configuration.CreateMap<DynamicEntityPropertyValue, DynamicEntityPropertyValueDto>().ReverseMap();

            //User Delegations
            configuration.CreateMap<CreateUserDelegationDto, UserDelegation>();

            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
        }
    }
}