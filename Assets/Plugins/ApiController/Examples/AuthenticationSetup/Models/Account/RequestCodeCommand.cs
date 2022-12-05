
namespace ApiController.Examples.Models
{
    public sealed class RequestCodeCommand
    {
        public RequestCodeCommand(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public string PhoneNumber { get; set; }
    }
}