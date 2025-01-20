using G_CustomerCommunication_API.Models;

namespace G_CustomerCommunication_API.BusinessLogics.Interfaces
{
    public interface ICommunication
    {
        Task<bool> SendNotificationAsync(SendNotifVM notifVM);
        Task<List<Notification>> GetNotificationsAsync(GetNotifVM notifVM);
        Task<>
    }
}