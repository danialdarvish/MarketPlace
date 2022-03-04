using MarketPlace.DataLayer.DTOs.Account;
using MarketPlace.DataLayer.Entities.Account;
using System; 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
        Task<bool> ChangeUserPassword(ChangePasswordDto changePass, long currentUserId);
        Task<EditUserProfileDto> GetProfileForEdit(long userId);
        Task<EditUserProfileResult> EditUserProfile(EditUserProfileDto profile, long userId, IFormFile avatarImage);

        #endregion
    }
}
