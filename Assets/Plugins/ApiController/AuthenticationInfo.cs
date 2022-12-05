using UnityEngine;

namespace ApiController
{
    public sealed class AuthenticationInfo
    {
        public string AccessToken
        {
            get => PlayerPrefs.GetString($"{nameof(AuthenticationInfo)}_{nameof(AccessToken)}", null);
            set => PlayerPrefs.SetString($"{nameof(AuthenticationInfo)}_{nameof(AccessToken)}", value);
        }

        public string RefreshToken
        {
            get => PlayerPrefs.GetString($"{nameof(AuthenticationInfo)}_{nameof(RefreshToken)}", null);
            set => PlayerPrefs.SetString($"{nameof(AuthenticationInfo)}_{nameof(RefreshToken)}", value);
        }
    }
}