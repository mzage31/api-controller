
using System;

namespace ApiController.Examples.Models
{
    public sealed class LoginResponse
    {
        public LoginResponse(string accessToken, string refreshToken, DateTime expireDate)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpireDate = expireDate;
        }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}