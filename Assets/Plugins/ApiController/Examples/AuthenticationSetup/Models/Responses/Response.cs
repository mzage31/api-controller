#nullable enable
using System;

namespace ApiController.Examples.Models
{
    [Serializable]
    public class Response<T> : IResponse
    {
        public Response()
        {
        }

        public Response(ApiResultCode code)
        {
            Code = code;
            if (Code != ApiResultCode.Ok && Code != ApiResultCode.GeneralError)
                Message = code.ToString("G");
        }

        public Response(ApiResultCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public Response(ApiResultCode code, T model)
        {
            Code = code;
            Model = model;
        }

        public Response(ApiResultCode code, string message, T model)
        {
            Code = code;
            Message = message;
            Model = model;
        }

        public ApiResultCode Code { get; set; }
        public string? Message { get; set; }
        public T? Model { get; set; }
    }

    [Serializable]
    public sealed class Response : Response<object>
    {
        public Response()
        {
        }

        public Response(ApiResultCode code) : base(code)
        {
        }

        public Response(ApiResultCode code, string message) : base(code, message)
        {
        }

        public Response(ApiResultCode code, object model) : base(code, model)
        {
        }
    }
}