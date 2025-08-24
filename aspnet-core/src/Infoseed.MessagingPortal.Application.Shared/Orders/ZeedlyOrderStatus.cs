
namespace Infoseed.MessagingPortal.Orders
{
    public enum ZeedlyOrderStatus
    {
        New = 1,
        Accepted = 2,
        UnderPrep = 3,
        Dispatched = 4,
        Rejected = 5,
        DriverOnTheWay = 6,
        OrderOnTheWay = 7, //DELIVERY_STARTED
        Delivered = 8, //DELIVERY_ENDED
        NSNA = 9 //CANCELED

    }
}
