using G_CustomerCommunication_API.BusinessLogics.Interfaces;
using G_CustomerCommunication_API.Models.Accounting;
using G_CustomerCommunication_API.Models.MiddlewareVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

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
            string host = _config.GetRequiredSection("MicroservicesUrl").GetValue<string>("Accounting")!;

            try
            {
                // BaseURL
                RestClient client = new($"{host}/api/User/GetUserInfo");
                RestRequest request = new()
                {
                    Method = Method.Post
                };

                // Parameters
                request.AddJsonBody(new { id = userId });

                // Headers
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");

                RestResponse response = await client.ExecutePostAsync(request);

                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
                {
                    ApiResponse? result = JsonConvert.DeserializeObject<ApiResponse?>(response.Content);
                    if (result != null && !string.IsNullOrEmpty(result.Data))
                        info = JsonConvert.DeserializeObject<UserInfo?>(result.Data);
                }
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
            string host = _config.GetRequiredSection("MicroservicesUrl").GetValue<string>("Accounting")!;

            try
            {
                // BaseURL
                RestClient client = new($"{host}/api/Attributes/GetUserInfo");
                RestRequest request = new()
                {
                    Method = Method.Post
                };

                // Parameters
                request.AddJsonBody(new { Token = token });

                // Headers
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");

                RestResponse response = await client.ExecutePostAsync(request);

                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
                {
                    ApiResponse? result = JsonConvert.DeserializeObject<ApiResponse?>(response.Content);
                    if (result != null && !string.IsNullOrEmpty(result.Data))
                        info = JsonConvert.DeserializeObject<UserInfo?>(result.Data);
                }
            }
            catch (Exception)
            {
                return new UserInfo();
            }

            return info;
        }
    }
}
