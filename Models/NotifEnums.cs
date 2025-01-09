namespace G_CustomerCommunication_API.Models
{
    public enum NotifTypes
    {
        SMS = 101,
        Email = 102,
        Dashboard = 103,
        Telegram = 104,
        OTP = 105
    }

    public enum NotifUnits
    {
        Accounting = 1,
        Wallet = 2,
        Store = 3,
        Panel = 4,
        IPG = 5,
        Basket = 6
    }
}
