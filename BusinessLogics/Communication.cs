using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Models;
using Microsoft.EntityFrameworkCore;

namespace G_CustomerCommunication_API.BusinessLogics
{
    public class Communication : ICommunication
    {
        private readonly ILogger<Communication> _logger;
        private readonly GCustomerCommunicationDbContext _customerComm;

        public Communication(GCustomerCommunicationDbContext customerComm, ILogger<Communication> logger)
        {
            _customerComm = customerComm;
            _logger = logger;
        }

        public async Task<List<Notification>> GetNotificationsAsync(GetNotifVM notifVM)
        {
            return await _customerComm.Notifications.Where(x => x.UserId == notifVM.UserId).ToListAsync();
        }

        public async Task<bool> SendNotificationAsync(SendNotifVM notifVM)
        {
            bool isOk = false;

            try
            {
                _customerComm.Notifications.Add(new Notification
                {
                    Body = notifVM.NotifBody,
                    NotificationLinkId = (long)notifVM.NotifTypes,
                    SenderUnit = notifVM.NotifUnit.ToString(),
                    UserId = notifVM.RecieverUserId!.Value,
                    SenderUserId = notifVM.SenderUserId!.Value,
                    DestinationAddress = notifVM.DestinationAddress!,
                    InsDate = DateTime.Now,
                    NotificationTemplateId = 0,
                    Status = 1,
                });
                await _customerComm.SaveChangesAsync();
                isOk = true;
            }
            catch (Exception)
            {
                isOk = false;
            }
            return isOk;
        }
    }
}
