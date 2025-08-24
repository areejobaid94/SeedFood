using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Infoseed.MessagingPortal.AccountBillings;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Authorization.Delegation;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Banks;
using Infoseed.MessagingPortal.Billings;
using Infoseed.MessagingPortal.BranchAreas;
using Infoseed.MessagingPortal.Branches;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.Chat;
using Infoseed.MessagingPortal.ChatStatuses;
using Infoseed.MessagingPortal.Cities;
using Infoseed.MessagingPortal.Close_Deals;
using Infoseed.MessagingPortal.CloseDealStatuses;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.ContactStatuses;
using Infoseed.MessagingPortal.Currencies;
using Infoseed.MessagingPortal.Customers;
using Infoseed.MessagingPortal.Deals;
using Infoseed.MessagingPortal.DealStatuses;
using Infoseed.MessagingPortal.DealTypes;
using Infoseed.MessagingPortal.DeliveryChanges;
using Infoseed.MessagingPortal.Editions;
using Infoseed.MessagingPortal.Evaluations;
using Infoseed.MessagingPortal.Forcasts;
using Infoseed.MessagingPortal.Friendships;
using Infoseed.MessagingPortal.Genders;
using Infoseed.MessagingPortal.InfoSeedServices;
using Infoseed.MessagingPortal.ItemAdditionsCategorys;
using Infoseed.MessagingPortal.ItemAndAdditionsCategorys;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.ItemSpecificationsDetails;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.MenuDetails;
using Infoseed.MessagingPortal.MenuItemStatuses;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.MultiTenancy.Accounting;
using Infoseed.MessagingPortal.MultiTenancy.Payments;
using Infoseed.MessagingPortal.OrderDetails;
using Infoseed.MessagingPortal.OrderLineAdditionalIngredients;
using Infoseed.MessagingPortal.OrderReceipts;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.OrderStatuses;
using Infoseed.MessagingPortal.PaymentMethods;
using Infoseed.MessagingPortal.ReceiptDetails;
using Infoseed.MessagingPortal.Receipts;
using Infoseed.MessagingPortal.ServiceFrequencies;
using Infoseed.MessagingPortal.ServiceStatuses;
using Infoseed.MessagingPortal.ServiceTypes;
using Infoseed.MessagingPortal.Storage;
using Infoseed.MessagingPortal.TemplateMessagePurposes;
using Infoseed.MessagingPortal.TemplateMessages;
using Infoseed.MessagingPortal.TenantServices;
using Infoseed.MessagingPortal.Territories;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.EntityFrameworkCore
{
    public class MessagingPortalDbContext : AbpZeroDbContext<Tenant, Role, User, MessagingPortalDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<Close_Deal> Close_Deals { get; set; }

        public virtual DbSet<CloseDealStatus> CloseDealStatuses { get; set; }

        public virtual DbSet<Deal> Deals { get; set; }

        public virtual DbSet<DealType> DealTypes { get; set; }

        public virtual DbSet<DealStatus> DealStatuses { get; set; }

        public virtual DbSet<Forcats> Forcatses { get; set; }

        public virtual DbSet<Territory> Territories { get; set; }

        public virtual DbSet<Billing> Billings { get; set; }

        public virtual DbSet<Contact> Contacts { get; set; }

        public virtual DbSet<ChatStatuse> ChatStatuses { get; set; }

        public virtual DbSet<ContactStatuse> ContactStatuses { get; set; }

        public virtual DbSet<Receipt> Receipts { get; set; }

        public virtual DbSet<ReceiptDetail> ReceiptDetails { get; set; }

        public virtual DbSet<Bank> Banks { get; set; }

        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

        public virtual DbSet<TenantService> TenantServices { get; set; }

        public virtual DbSet<AccountBilling> AccountBillings { get; set; }

        public virtual DbSet<Currency> Currencies { get; set; }

        public virtual DbSet<InfoSeedService> InfoSeedServices { get; set; }

        public virtual DbSet<ServiceFrquency> ServiceFrquencies { get; set; }

        public virtual DbSet<ServiceStatus> ServiceStatuses { get; set; }

        public virtual DbSet<ServiceType> ServiceTypes { get; set; }

        public virtual DbSet<TemplateMessage> TemplateMessages { get; set; }

        public virtual DbSet<TemplateMessagePurpose> TemplateMessagePurposes { get; set; }

        public virtual DbSet<OrderReceipt> OrderReceipts { get; set; }

        public virtual DbSet<OrderLineAdditionalIngredient> OrderLineAdditionalIngredients { get; set; }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<ExtraOrderDetails.ExtraOrderDetail> ExtraOrderDetails { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

        public virtual DbSet<Gender> Genders { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Branch> Branches { get; set; }

        public virtual DbSet<Evaluation> Evaluations { get; set; }

        public virtual DbSet<DeliveryChange> DeliveryChanges { get; set; }

        public virtual DbSet<BranchArea> BranchAreas { get; set; }

        public virtual DbSet<Area> Areas { get; set; }

        public virtual DbSet<MenuDetail> MenuDetails { get; set; }

        public virtual DbSet<Item> Items { get; set; }

        public virtual DbSet<ItemAdditions.ItemAdditions> ItemAdditions { get; set; }

        public virtual DbSet<Menu> Menus { get; set; }

        public virtual DbSet<ItemCategory> ItemCategorys { get; set; }

        public virtual DbSet<MenuItemStatus> MenuItemStatuses { get; set; }
        //public virtual DbSet<Location> Locations { get; set; }

        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

        public virtual DbSet<UserDelegation> UserDelegations { get; set; }

        public virtual DbSet<TenantInformation.TenantInformation> TenantInformation { get; set; }
        public virtual DbSet<ContactNotification.ContactNotification> ContactNotification { get; set; }

        public virtual DbSet<Caption> Captions { get; set; }
        public virtual DbSet<LanguageBot> LanguageBots { get; set; }
        public virtual DbSet<TextResource> TextResources { get; set; }

        public virtual DbSet<DeliveryLocationCost.DeliveryLocationCost> DeliveryLocationCosts { get; set; }
        public virtual DbSet<DeliveryOrderDetails.DeliveryOrderDetails> DeliveryOrderDetailss { get; set; }

        public virtual DbSet<OrderOffers.OrderOffer> OrderOffers { get; set; }

        public virtual DbSet<SalesUserCreate.SalesUserCreate> SalesUserCreates { get; set; }

        public virtual DbSet<ItemAdditionsCategory>  ItemAdditionsCategories { get; set; }

        public virtual DbSet<ItemAndAdditionsCategory> ItemAndAdditionsCategorys { get; set; }

        public virtual DbSet<Specifications.Specification> Specifications { get; set; }
        public virtual DbSet<SpecificationChoices.SpecificationChoice> SpecificationChoices { get; set; }
        public virtual DbSet<ItemSpecifications.ItemSpecification> ItemSpecifications { get; set; }

        public virtual DbSet<ItemSpecificationsDetail> ItemSpecificationsDetails { get; set; }
        public MessagingPortalDbContext(DbContextOptions<MessagingPortalDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Deal>(d =>
            {
                d.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<DealType>(d =>
                       {
                           d.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<AccountBilling>(a =>
                       {
                           a.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Billing>(b =>
                       {
                           b.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<TemplateMessage>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Contact>(c =>
                       {
                           c.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Receipt>(r =>
                       {
                           r.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<TenantService>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<AccountBilling>(a =>
                       {
                           a.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<TemplateMessage>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<OrderReceipt>(o =>
                       {
                           o.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<OrderLineAdditionalIngredient>(o =>
                       {
                           o.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<OrderDetail>(o =>
                       {
                           o.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Order>(o =>
                       {
                           o.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Customer>(c =>
                       {
                           c.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<OrderStatus>(o =>
                       {
                           o.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Gender>(g =>
                       {
                           g.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<City>(c =>
                       {
                           c.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Branch>(b =>
                       {
                           b.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<DeliveryChange>(d =>
                       {
                           d.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<BranchArea>(b =>
                       {
                           b.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Area>(a =>
                       {
                           a.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<MenuDetail>(m =>
                       {
                           m.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Item>(i =>
                       {
                           i.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Menu>(m =>
                       {
                           m.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<ItemCategory>(m =>
                       {
                           m.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<BinaryObject>(b =>
                       {
                           b.HasIndex(e => new { e.TenantId });
                       });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.Entity<SubscriptionPaymentExtensionData>(b =>
            {
                b.HasQueryFilter(m => !m.IsDeleted)
                    .HasIndex(e => new { e.SubscriptionPaymentId, e.Key, e.IsDeleted })
                    .IsUnique();
            });

            modelBuilder.Entity<UserDelegation>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.SourceUserId });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId });
            });

            modelBuilder.Entity<TenantInformation.TenantInformation>(c =>
            {
                c.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}