namespace G_CustomerCommunication_API.Models
{
    public class SendNotifVM
    {
        public NotifTypes NotifTypes { get; set; }
        public string? NotifBody { get; set; }
        public long? SenderUserId { get; set; }
        public long? RecieverUserId { get; set; }
        public NotifUnits NotifUnit { get; set; }
        public string? DestinationAddress { get; set; }
    }

    public class GetNotifVM
    {
        public long? UserId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public NotifUnits NotifUnit { get; set; }
        public NotifTypes NotifTypes { get; set; }
    }
}
