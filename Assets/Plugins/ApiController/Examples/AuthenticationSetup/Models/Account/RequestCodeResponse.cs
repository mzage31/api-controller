

namespace ApiController.Examples.Models
{
    public sealed class RequestCodeResponse
    {
        public RequestCodeResponse(int loginId, int code)
        {
            LoginId = loginId;
            Code = code;
        }

        public int LoginId { get; set; }
        public int Code { get; set; }
    }
}