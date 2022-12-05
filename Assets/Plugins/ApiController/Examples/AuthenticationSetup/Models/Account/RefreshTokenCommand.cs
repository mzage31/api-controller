
namespace ApiController.Examples.Models
{
    public sealed class RefreshTokenCommand
    {
        public RefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public string RefreshToken { get; set; }
    }
}