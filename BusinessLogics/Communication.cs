using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Models;
using G_CustomerCommunication_API.Models.Accounting;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<SurveyQuestionsVM>?> GetSurveyQuestionsAsync(SurveyFiltersVM surveyFilters)
        {
            List<SurveyQuestionsVM>? questions = await _customerComm
                .SurveyTemplates
                .SelectMany(st => _customerComm
                .SurveyTemplateQuestions
                .Where(x => x.SurveyTemplateId == st.Id)
                /*.DefaultIfEmpty()*/, (st, stq) => new { st, stq })
                .Where(x => x.st.Station == surveyFilters.StationId && x.st.NotificationLinkId == surveyFilters.NotificationLinkId)
                .Select(x => new SurveyQuestionsVM()
                {
                    AnswerType = x.stq.ValueType,
                    Question = x.stq.Question,
                    SurveyTemplateId = x.stq.SurveyTemplateId,
                })
                .ToListAsync();

            return questions;
        }

        public async Task<bool> RegisterUserSurveyAsync(SurveyAnswersVM surveyAnswers)
        {
            bool isOk = false;
            try
            {
                bool isValid = surveyAnswers != null &&
                    surveyAnswers.SurveyTemplateId != null &&
                    surveyAnswers.UserId != null &&
                    surveyAnswers.UserId > 0 &&
                    surveyAnswers.SurveyTemplateId > 0 &&
                    !string.IsNullOrEmpty(surveyAnswers.Answers);

                if (isValid)
                {
                    Survey survey = new Survey()
                    {
                        QuestionValues = surveyAnswers!.Answers!,
                        RegDate = DateTime.Now,
                        SurveyTemplateId = surveyAnswers.SurveyTemplateId!.Value,
                        UserId = surveyAnswers.UserId!.Value
                    };
                    await _customerComm.Surveys.AddAsync(survey);
                    await _customerComm.SaveChangesAsync();
                    isOk = true;
                }
            }
            catch (Exception)
            {
                isOk = false;
            }
            return isOk;
        }

        public async Task<bool> SendNotificationAsync(SendNotifVM notifVM)
        {
            bool isOk = false;
            bool isSended = false;
            string? destinationAddress = string.Empty;
            UserInfo? userInfo = new();

            if (!string.IsNullOrEmpty(notifVM.Token))
                userInfo = await _accounting.GetUserInfoByTokenAsync(notifVM.Token!);
            else if (notifVM.RecieverUserId != null && notifVM.RecieverUserId > 0)
                userInfo = await _accounting.GetUserInfoByIdAsync(notifVM.RecieverUserId);

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
                            destinationAddress = userInfo.Mobile;
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
