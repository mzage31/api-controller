#nullable enable
namespace ApiController.Examples.Models
{
    public interface IResponse
    {
        public ApiResultCode Code { get; set; }
        public string? Message { get; set; }
    }
}