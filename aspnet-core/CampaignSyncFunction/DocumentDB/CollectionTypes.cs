namespace CampaignSyncFunction
{
    public enum CollectionTypes
    {
       ItemsCollection,
       CustomersCollection,
       ConversationsCollection,
    }

    public enum CustomerStatus
    {
        Active =1,
        InActive,
    }
    public enum CustomerChatStatus
    {
        All=0,
        Active = 1,
        ClosedChats =2,
    }

    public enum InfoSeedContainerItemTypes
    {
        CustomerItem,
        ConversationItem,
        Tenant,
        ConversationBot,
        BotFlow,
        Campaign
    }
}