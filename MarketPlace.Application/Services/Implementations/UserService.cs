using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Account;
using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Application.Extensions;
using MarketPlace.Application.Utils;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        #region Constructor
        private readonly IPasswordHelper _passwordHelper;
        private readonly IGenericRepository<User> _userRepository;
        public UserService(IGenericRepository<User> userRepository, IPasswordHelper passwordHelper)
        {
            _userRepository = userRepository;
            _passwordHelper = passwordHelper;
        }
        #endregion

        #region Account
        public async Task<RegisterUserResult> RegisterUser(RegisterUserDto register)
        {
            if (!await IsUserExistByMobileNumber(register.Mobile))
            {
                var user = new User
                {
                    Mobile = register.Mobile,
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    Password = _passwordHelper.EncodePassswordMd5(register.Password),
                    MobileActiveCode = new Random().Next(10000, 999999).ToString(),
                    EmailActiveCode = Guid.NewGuid().ToString("N")
                };

                await _userRepository.AddEntity(user);
                await _userRepository.SaveChanges();
                // todo: Send activation mobile code to user

                return RegisterUserResult.Success;
            }

            return RegisterUserResult.MobileExists;
        }

        public async Task<bool> IsUserExistByMobileNumber(string mobile)
        {
            return await _userRepository.GetQuery().AnyAsync(x => x.Mobile == mobile);
        }


        public async Task<LoginUserResult> GetUserForLogin(LoginUserDto login)
        {
            var user = await _userRepository.GetQuery().FirstOrDefaultAsync(x => x.Mobile == login.Mobile);
            if (user == null) return LoginUserResult.NotFound;
            if (!user.IsMobileActive) return LoginUserResult.NotActivated;
            if (user.Password != _passwordHelper.EncodePassswordMd5(login.Password)) return LoginUserResult.NotFound;

            return LoginUserResult.Success;
        }

        public async Task<User> GetUserByMobile(string mobile)
        {
            return await _userRepository.GetQuery().FirstOrDefaultAsync(x => x.Mobile == mobile);
        }


        public async Task<ForgotPasswordResult> RecoverUserPassword(ForgotPasswordDto forgot)
        {
            var user = await _userRepository.GetQuery().FirstOrDefaultAsync(x => x.Mobile == forgot.Mobile);
            if (user == null) return ForgotPasswordResult.NotFound;
            var newPassword = new Random().Next(1000000, 99999999).ToString();
            user.Password = _passwordHelper.EncodePassswordMd5(newPassword);
            _userRepository.EditEntity(user);
            // todo: Send new password to user with SMS

            await _userRepository.SaveChanges();

            return ForgotPasswordResult.Success;
        }

        public async Task<bool> ChangeUserPassword(ChangePasswordDto changePass, long currentUserId)
        {
            var user = await _userRepository.GetEntityById(currentUserId);
            if (user != null)
            {
                var newPassword = _passwordHelper.EncodePassswordMd5(changePass.NewPassword);
                if (newPassword != user.Password)
                {
                    user.Password = newPassword;
                    _userRepository.EditEntity(user);
                    await _userRepository.SaveChanges();

                    return true;
                }
            }
            return false;
        }

        public async Task<EditUserProfileDto> GetProfileForEdit(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            if (user == null) return null;

            return new EditUserProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar
            };
        }

        public async Task<EditUserProfileResult> EditUserProfile(EditUserProfileDto profile, long userId, IFormFile avatarImage)
        {
            var user = await _userRepository.GetEntityById(userId);

            if (user == null) return EditUserProfileResult.NotFound;
            if (user.IsBlocked) return EditUserProfileResult.IsBlocked;
            if (!user.IsMobileActive) return EditUserProfileResult.IsNotActive;

            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;

            if (avatarImage != null && avatarImage.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(avatarImage.FileName);
                avatarImage.AddImageToServer(imageName, PathExtension.UserAvatarOriginServer, 100, 100, PathExtension.UserAvatarThumbServer, user.Avatar);
                user.Avatar = imageName;
            }

            _userRepository.EditEntity(user);
            await _userRepository.SaveChanges();

            return EditUserProfileResult.Success;
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await _userRepository.DisposeAsync();
        }

        #endregion
    }
}
