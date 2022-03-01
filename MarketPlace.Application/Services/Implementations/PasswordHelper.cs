using MarketPlace.Application.Services.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MarketPlace.Application.Services.Implementations
{
    public class PasswordHelper : IPasswordHelper
    {
        public string EncodePassswordMd5(string password)
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            originalBytes  =ASCIIEncoding.Default.GetBytes(password);
            encodedBytes = md5.ComputeHash(originalBytes);
            return BitConverter.ToString(encodedBytes);
        }
    }
}
