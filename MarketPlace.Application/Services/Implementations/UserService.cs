﻿using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Account;
using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        #endregion

        #region Dispose
        public async ValueTask DisposeAsync()
        {
            await _userRepository.DisposeAsync();
        }
        #endregion
    }
}