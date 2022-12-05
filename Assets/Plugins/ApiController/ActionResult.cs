#nullable enable
using System.Net;
using Newtonsoft.Json;

namespace ApiController
{
    public class ActionResult<T> : IActionResult where T : class
    {
        public bool Handled { get; set; }
        public HttpStatusCode StatusCode { get; }
        [JsonIgnore] public object? Object => Response;
        public T? Response { get; }

        public ActionResult(HttpStatusCode statusCode, T? response)
        {
            Handled = false;
            StatusCode = statusCode;
            Response = response;
        }
    }
}