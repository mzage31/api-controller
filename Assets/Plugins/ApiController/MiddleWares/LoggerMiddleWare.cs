using Newtonsoft.Json;
using UnityEngine;

namespace ApiController.MiddleWares
{
    public class LoggerMiddleWare : IRequestMiddleWare
    {
        public void Invoke<T>(string address, ActionResult<T> result) where T : class
        {
            if (result.Handled)
                return;
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.Log($"LoggerMiddleWare ({address}):\n{json}");
        }
    }
}