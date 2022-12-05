using System.Net;
using ApiController.Examples.Models;
using UnityEngine;

namespace ApiController.Examples.MiddleWares
{
    public class GeneralErrorMiddleWare : IRequestMiddleWare
    {
        public void Invoke<T>(string address, ActionResult<T> result) where T : class
        {
            if (result.Handled)
                return;
            if (result.StatusCode == HttpStatusCode.OK && result.Response is IResponse { Code: ApiResultCode.GeneralError } ir)
            {
                Debug.LogError($"GeneralError in {address}\n{ir.Message}");
                result.Handled = true;
            }
        }
    }
}