namespace G_CustomerCommunication_API.Models
{
    public class SendNotifVM
    {
        public NotifTypes NotifTypes { get; set; }
        public string? NotifBody { get; set; }
        public long? SenderUserId { get; set; }
        public NotifUnits NotifUnit { get; set; }
        public string? DestinationAddress { get; set; }
        public string? Token { get; set; }
    }

    public class GetNotifVM
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public NotifUnits NotifUnit { get; set; }
        public NotifTypes NotifTypes { get; set; }
        public string? Token { get; set; }
    }
}
