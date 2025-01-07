using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Models;
using G_CustomerCommunication_API.Models.MiddlewareVM;
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
            if (notifVM != null && notifVM.SenderUserId != 0 && !string.IsNullOrEmpty(notifVM.Token))
            {
                bool isSended = await _customerComm.SendNotificationAsync(notifVM);
                return Ok(new ApiResponse(isSended ? 200 : 400, data: isSended.ToString().ToLower()));
            }
            return BadRequest(new ApiResponse(404));
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
                    return Ok(new ApiResponse(200, data: jsonData));
                }
            }
            return BadRequest(new ApiResponse(404));
        }
    }
}