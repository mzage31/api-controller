#nullable enable
using System.Net;

namespace ApiController
{
    public sealed class ObjectActionResult : ActionResult<object>
    {
        public ObjectActionResult(HttpStatusCode statusCode, object? response) : base(statusCode, response)
        {
        }
    }
}