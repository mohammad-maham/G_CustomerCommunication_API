using G_CustomerCommunication_API.Models.Accounting;

namespace G_CustomerCommunication_API.BusinessLogics.Interfaces
{
    public interface IAccounting
    {
        Task<UserInfo?> GetUserInfoByTokenAsync(string token);
    }
}