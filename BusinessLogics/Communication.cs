using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Models;
using G_CustomerCommunication_API.Models.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;

namespace G_CustomerCommunication_API.BusinessLogics
{
    public class Communication : ICommunication
    {
        private readonly ILogger<Communication> _logger;
        private readonly GCustomerCommunicationDbContext _customerComm;
        private readonly INotificationManager _notificationManager;
        private readonly IAccounting _accounting;

        public Communication(GCustomerCommunicationDbContext customerComm, ILogger<Communication> logger, INotificationManager notificationManager, IAccounting accounting)
        {
            _customerComm = customerComm;
            _logger = logger;
            _notificationManager = notificationManager;
            _accounting = accounting;
        }

        public async Task<List<Notification>> GetNotificationsAsync(GetNotifVM notifVM)
        {
            IQueryable<Notification> notifs = null!;
            try
            {
                UserInfo? userInfo = await _accounting.GetUserInfoByTokenAsync(notifVM.Token!);
                if (userInfo != null && userInfo.Id > 0)
                {
                    long userId = userInfo.Id;
                    notifs = _customerComm.Notifications
                        .Where(x =>
                        x.UserId == userId
                        && x.InsDate >= notifVM.FromDate
                        && x.InsDate <= notifVM.ToDate
                        && x.NotificationLinkId == (long)notifVM.NotifTypes
                        && x.DestinationAddress == notifVM.NotifUnit.ToString());
                }
            }
            catch (Exception)
            {
                return new List<Notification>();
            }

            return await notifs.ToListAsync();
        }

        public async Task<bool> SendNotificationAsync(SendNotifVM notifVM)
        {
            bool isOk = false;
            bool isSended = false;
            string? destinationAddress = string.Empty;

            UserInfo? userInfo = await _accounting.GetUserInfoByTokenAsync(notifVM.Token!);
            if (userInfo != null && userInfo.Id > 0)
            {
                long userId = userInfo.Id;
                try
                {
                    switch (notifVM.NotifTypes)
                    {
                        case NotifTypes.SMS:
                        case NotifTypes.OTP:
                            isSended = await _notificationManager.SendSMSNotifAsync(notifVM);
                            destinationAddress = userInfo.Mobile.ToString();
                            break;
                        case NotifTypes.Email:
                            isSended = await _notificationManager.SendEmailNotifAsync(notifVM);
                            destinationAddress = userInfo.Email?.ToString();
                            break;
                        case NotifTypes.Dashboard:
                        case NotifTypes.Telegram:
                            isSended = true;
                            destinationAddress = "Dash/Tel";
                            break;
                        default:
                            break;
                    }
                    string notifRes = JsonConvert.SerializeObject(new { Status = isSended ? 200 : 0, Sended = isSended.ToString().ToLower() });
                    _customerComm.Notifications.Add(new Notification
                    {
                        Body = notifVM.NotifBody,
                        NotificationLinkId = (long)notifVM.NotifTypes,
                        SenderUnit = notifVM.NotifUnit.ToString(),
                        UserId = userId,
                        SenderUserId = notifVM.SenderUserId!.Value,
                        DestinationAddress = /*notifVM.DestinationAddress!*/destinationAddress!,
                        InsDate = DateTime.Now,
                        NotificationTemplateId = 0,
                        Status = isSended ? 200 : 0,
                        NotificationResult = notifRes
                    });
                    await _customerComm.SaveChangesAsync();
                    isOk = true;
                }
                catch (Exception)
                {
                    string notifRes = JsonConvert.SerializeObject(new { Status = 500, Sended = isSended.ToString().ToLower() });
                    _customerComm.Notifications.Add(new Notification
                    {
                        Body = notifVM.NotifBody,
                        NotificationLinkId = (long)notifVM.NotifTypes,
                        SenderUnit = notifVM.NotifUnit.ToString(),
                        UserId = userId,
                        SenderUserId = notifVM.SenderUserId!.Value,
                        DestinationAddress = /*notifVM.DestinationAddress!*/destinationAddress!,
                        InsDate = DateTime.Now,
                        NotificationTemplateId = 0,
                        Status = 500,
                        NotificationResult = notifRes,
                    });
                    await _customerComm.SaveChangesAsync();
                    isOk = false;
                }
            }

            return isOk;
        }
    }
}
