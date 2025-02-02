using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Models.Accounting;
using GoldHelpers.Helpers;
using GoldHelpers.Models;
using Newtonsoft.Json;

namespace G_CustomerCommunication_API.BusinessLogics
{
    public class Accounting : IAccounting
    {
        private readonly ILogger<Accounting> _logger;
        private readonly IConfiguration _config;

        public Accounting(ILogger<Accounting> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<UserInfo?> GetUserInfoByIdAsync(long? userId)
        {
            UserInfo? info = new();

            try
            {
                GoldAPIResult? result = await new GoldAPIResponse(GoldHosts.Accounting, "/api/User/GetUserInfoById", new { id = userId }).PostAsync();

                if (result != null && !string.IsNullOrEmpty(result.Data))
                    info = JsonConvert.DeserializeObject<UserInfo?>(result.Data);
            }
            catch (Exception)
            {
                return new UserInfo();
            }

            return info;
        }

        public async Task<UserInfo?> GetUserInfoByTokenAsync(string token)
        {
            UserInfo? info = new();

            try
            {
                GoldAPIResult? result = await new GoldAPIResponse(GoldHosts.Accounting, "/api/Attributes/GetUserInfo", new { Token = token }).PostAsync();

                if (result != null && !string.IsNullOrEmpty(result.Data))
                    info = JsonConvert.DeserializeObject<UserInfo?>(result.Data);
            }
            catch (Exception)
            {
                return new UserInfo();
            }

            return info;
        }
    }
}
