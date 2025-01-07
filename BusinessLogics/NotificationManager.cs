using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Models;
using G_CustomerCommunication_API.Models.Accounting;
using IPE.SmsIrClient;
using IPE.SmsIrClient.Models.Results;
using System.Net;
using System.Net.Mail;

namespace G_CustomerCommunication_API.BusinessLogics
{
    public class NotificationManager : INotificationManager
    {
        private readonly IConfiguration _config;
        private readonly ILogger<NotificationManager> _logger;
        private readonly IAccounting _accounting;

        public NotificationManager(ILogger<NotificationManager> logger, IConfiguration config, IAccounting accounting)
        {
            _logger = logger;
            _config = config;
            _accounting = accounting;
        }

        public async Task<bool> SendEmailNotifAsync(SendNotifVM notifVM)
        {
            IConfigurationSection? configs = _config.GetSection("SMTPOptions");

            bool isOk = false;
            int port = configs.GetValue<int>("Port")!;
            string host = configs.GetValue<string>("Host")!;
            string subject = configs.GetValue<string>("Subject")!;
            bool enableSSL = configs.GetValue<bool>("EnableSsl")!;
            string username = configs.GetValue<string>("Username")!;
            string password = configs.GetValue<string>("Password")!;
            string senderMail = configs.GetValue<string>("FromEmail")!;
            bool useDefaultCredentials = configs.GetValue<bool>("UseDefaultCredentials")!;

            try
            {
                if (notifVM != null)
                {
                    UserInfo? userInfo = await _accounting.GetUserInfoByTokenAsync(notifVM.Token!);
                    if (userInfo != null && !string.IsNullOrEmpty(userInfo.Email))
                    {
                        string recieverMail = userInfo.Email;
                        // Set up SMTP client
                        SmtpClient client = new(host, port)
                        {
                            EnableSsl = enableSSL,
                            UseDefaultCredentials = useDefaultCredentials,
                            Credentials = new NetworkCredential(username, password),
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                        };

                        // Create email message
                        MailMessage mailMessage = new()
                        {
                            From = new MailAddress(senderMail)
                        };
                        mailMessage.To.Add(recieverMail);
                        mailMessage.Subject = subject;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = notifVM.NotifBody;

                        // Send email
                        await client.SendMailAsync(mailMessage);
                        client.Dispose();
                        isOk = true;
                    }
                }
            }
            catch (Exception)
            {
                isOk = false;
            }

            return isOk;
        }

        public async Task<bool> SendSMSNotifAsync(SendNotifVM notifVM)
        {
            bool isOk = false;
            int? sendDateTime = null; // unix time - for instance: 1704094200
            IConfigurationSection? configs = _config.GetSection("SMSOptions");
            string api_key = configs.GetValue<string>("key")!;
            long lineNumber = configs.GetValue<long>("linenumber")!;
            SmsIr smsIr = new(api_key);

            try
            {
                if (notifVM != null)
                {
                    UserInfo? userInfo = await _accounting.GetUserInfoByTokenAsync(notifVM.Token!);
                    if (userInfo != null && userInfo.Mobile != null && userInfo.Mobile > 0)
                    {
                        string mobile = $"0{userInfo.Mobile.ToString()!}";
                        SmsIrResult<SendResult> response = await smsIr.BulkSendAsync(lineNumber, notifVM.NotifBody, [mobile], sendDateTime);

                        SendResult sendResult = response.Data;
                        Guid packId = sendResult.PackId;
                        int?[] messageIds = sendResult.MessageIds;
                        decimal cost = sendResult.Cost;

                        _logger.LogInformation($"SMS msgId: {messageIds}");
                        isOk = messageIds.Length > 0;
                    }
                }
            }
            catch (Exception)
            {
                isOk = false;
            }

            return isOk;
        }
    }
}
