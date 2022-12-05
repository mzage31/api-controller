using System.Threading.Tasks;
using ApiController.Attributes;
using ApiController.Examples.Authentication;
using ApiController.Examples.MiddleWares;
using ApiController.Examples.Models;
using ApiController.MiddleWares;

namespace ApiController.Examples.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class AccountController : Controller<AccountController>
    {
        public AccountController()
        {
            AddAuthentication<JwtAuthenticator>();
            AddMiddleWare<LoggerMiddleWare>();
            AddMiddleWare<GeneralErrorMiddleWare>();
        }

        [HttpPost, AllowAnonymous]
        public static Task<ActionResult<Response<RequestCodeResponse>>> RequestCode(RequestCodeCommand command)
        {
            return Send<Response<RequestCodeResponse>>(command);
        }

        [HttpPost, AllowAnonymous]
        public static Task<ActionResult<Response<LoginResponse>>> Login(LoginCommand command)
        {
            return Send<Response<LoginResponse>>(command);
        }

        [HttpPost, AllowAnonymous]
        public static Task<ActionResult<Response<RefreshTokenResponse>>> RefreshToken(RefreshTokenCommand command)
        {
            return Send<Response<RefreshTokenResponse>>(command);
        }

        [HttpPost, Authorize]
        public static Task<ActionResult<Response>> SetDisplayName(SetDisplayNameCommand command)
        {
            return Send<Response>(command);
        }
    }
}