using System;
using System.Net;
using System.Threading.Tasks;
using ApiController.Examples.Controllers;
using ApiController.Examples.Models;
using UnityEngine;

namespace ApiController.Examples
{
    public class Test2 : MonoBehaviour
    {
        [Multiline(5), SerializeField] private string instruction;

        private void OnValidate()
        {
            instruction = "Please use a valid url supporting\nJWT authentication with the\ncurrent implementation for this to work.";
        }

        private async void Start()
        {
            await RequestCode();
        }

        private static async Task RequestCode()
        {
            var result = await AccountController.RequestCode(new RequestCodeCommand("01234567890"));
            if (result.Handled)
                return;
            if (result is { StatusCode: HttpStatusCode.OK, Response: { Code: ApiResultCode.Ok, Model: { } requestCodeResponse } })
                await Login(requestCodeResponse);
            else if (result.StatusCode != HttpStatusCode.OK)
                Debug.LogError($"Http Error: {result.StatusCode:G} ({(int)result.StatusCode})");
            else
                Debug.LogError($"Api Error : {result.Response?.Message ?? "No message"}");
        }

        private static async Task Login(RequestCodeResponse requestCodeResponse)
        {
            var result = await AccountController.Login(new LoginCommand { Code = requestCodeResponse.Code, LoginId = requestCodeResponse.LoginId });
            if (result.Handled)
                return;
            if (result is { StatusCode: HttpStatusCode.OK, Response: { Code: ApiResultCode.Ok, Model: { } loginResponse } })
            {
                Debug.Log(loginResponse.AccessToken);
            }
            else if (result.StatusCode != HttpStatusCode.OK)
                Debug.LogError($"Http Error: {result.StatusCode:G} ({(int)result.StatusCode})");
            else
                Debug.LogError($"Api Error : {result.Response?.Message ?? "No message"}");
        }
    }
}