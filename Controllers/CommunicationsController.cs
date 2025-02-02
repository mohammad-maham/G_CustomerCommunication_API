using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Models;
using GoldHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace G_CustomerCommunication_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicationsController : ControllerBase
    {
        private readonly ILogger<CommunicationsController> _logger;
        private readonly ICommunication _customerComm;

        public CommunicationsController(ILogger<CommunicationsController> logger, ICommunication customerComm)
        {
            _logger = logger;
            _customerComm = customerComm;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotifVM notifVM)
        {
            if (notifVM != null && notifVM.SenderUserId != 0 && (!string.IsNullOrEmpty(notifVM.Token)
                || (notifVM.RecieverUserId != null && notifVM.RecieverUserId > 0)))
            {
                bool isSended = await _customerComm.SendNotificationAsync(notifVM);
                return Ok(new GoldAPIResult(isSended ? 200 : 400, data: isSended.ToString().ToLower()));
            }
            return BadRequest(new GoldAPIResult(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetNotifications([FromBody] GetNotifVM notifVM)
        {
            if (notifVM != null && !string.IsNullOrEmpty(notifVM.Token))
            {
                List<Notification> notifs = await _customerComm.GetNotificationsAsync(notifVM);
                if (notifs.Count > 0)
                {
                    string jsonData = JsonConvert.SerializeObject(notifs);
                    return Ok(new GoldAPIResult(200, data: jsonData));
                }
            }
            return BadRequest(new GoldAPIResult(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetSurveyQuestions([FromBody] SurveyFiltersVM surveyTemplate)
        {
            if (surveyTemplate != null && surveyTemplate.StationId > 0 && surveyTemplate.NotificationLinkId > 0)
            {
                List<SurveyQuestionsVM>? questions = await _customerComm.GetSurveyQuestionsAsync(surveyTemplate);
                if (questions != null && questions.Count > 0)
                {
                    string jsonData = JsonConvert.SerializeObject(questions);
                    return Ok(new GoldAPIResult(data: jsonData));
                }
            }
            return BadRequest(new GoldAPIResult(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SubmitUserSurvey([FromBody] SurveyAnswersVM surveyTemplate)
        {
            if (surveyTemplate != null &&
                surveyTemplate.UserId > 0 &&
                surveyTemplate.SurveyTemplateId > 0 &&
                !string.IsNullOrEmpty(surveyTemplate.Answers))
            {
                bool isRegistered = await _customerComm.RegisterUserSurveyAsync(surveyTemplate);
                return Ok(new GoldAPIResult(isRegistered ? 200 : 400));
            }
            return BadRequest(new GoldAPIResult(404));
        }
    }
}