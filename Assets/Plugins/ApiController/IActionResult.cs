#nullable enable
using System.Net;

namespace ApiController
{
    public interface IActionResult
    {
        public bool Handled { get; set; }
        public HttpStatusCode StatusCode { get; }
        public object? Object { get; }
    }
}