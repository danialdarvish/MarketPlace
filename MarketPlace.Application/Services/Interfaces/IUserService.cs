using MarketPlace.DataLayer.DTOs.Account;
using System; 
using System.Threading.Tasks;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IUserService : IAsyncDisposable
    {
        #region Account

        Task<RegisterUserResult> RegisterUser(RegisterUserDto register);
        Task<bool> IsUserExistByMobileNumber(string mobile);

        #endregion
    }
}
