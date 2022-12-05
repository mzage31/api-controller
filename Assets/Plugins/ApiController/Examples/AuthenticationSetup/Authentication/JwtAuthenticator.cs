using System.Net;
using ApiController.Examples.Models;
using UnityEngine;

namespace ApiController.Examples.Authentication
{
    public class JwtAuthenticator : IAuthenticator
    {
        public void Authenticate<T>(AuthenticationInfo info, ActionResult<T> result) where T : class
        {
            if (result.StatusCode == HttpStatusCode.OK)
            {
                switch (result.Response)
                {
                    case Response<LoginResponse> { Code: ApiResultCode.Ok, Model: { } } r1:
                        Debug.Log($"Authenticating: {r1.Model.RefreshToken}");
                        info.AccessToken = r1.Model.AccessToken;
                        info.RefreshToken = r1.Model.RefreshToken;
                        break;
                    case Response<RefreshTokenResponse> { Code: ApiResultCode.Ok, Model: { } } r2:
                        Debug.Log($"Authenticating: {r2.Model.RefreshToken}");
                        info.AccessToken = r2.Model.AccessToken;
                        info.RefreshToken = r2.Model.RefreshToken;
                        break;
                }
            }
        }
    }
}