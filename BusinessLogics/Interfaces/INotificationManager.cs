using G_CustomerCommunication_API.Models;

namespace G_CustomerCommunication_API.BusinessLogics.Interfaces
{
    public interface INotificationManager
    {
        Task<bool> SendSMSNotifAsync(SendNotifVM notifVM);
        Task<bool> SendEmailNotifAsync(SendNotifVM notifVM);
    }
}