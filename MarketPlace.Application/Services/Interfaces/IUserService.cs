using MarketPlace.DataLayer.DTOs.Account;
using MarketPlace.DataLayer.Entities.Account;
using System; 
using System.Threading.Tasks;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IUserService : IAsyncDisposable
    {
        #region Account

        Task<RegisterUserResult> RegisterUser(RegisterUserDto register);
        Task<bool> IsUserExistByMobileNumber(string mobile);
        Task<LoginUserResult> GetUserForLogin(LoginUserDto login);
        Task<User> GetUserByMobile(string mobile);
        Task<ForgotPasswordResult> RecoverUserPassword(ForgotPasswordDto forgot);
        #endregion
    }
}
