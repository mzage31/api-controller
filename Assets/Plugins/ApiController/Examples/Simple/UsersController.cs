using System.Threading.Tasks;
using ApiController.Attributes;
using ApiController.MiddleWares;

namespace ApiController.Examples
{
    [ApiController, Route("[controller]/[action]")]
    public class UsersController : Controller<UsersController>
    {
        public UsersController()
        {
            AddMiddleWare<LoggerMiddleWare>();
        }
        
        // Request: baseUrl/Users/1 [GET]
        // Request action name is assigned in the HttpGet Attribute
        // Return type is "IActionResult" because the model doesn't matter
        // Anyone can access this method anonymously
        [HttpGet("1"), AllowAnonymous]
        public static async Task<IActionResult> GetUser1()
        {
            return await Send<string>();
        }
        
        // Request: baseUrl/Users/2 [GET]
        // Request action name is assigned in the HttpGet Attribute
        // Return type is "ActionResult<T>" because the response in that result is defined as "T"
        // Anyone can access this method anonymously
        [HttpGet("2"), AllowAnonymous]
        public static async Task<ActionResult<UserModel>> GetUser2()
        {
            return await Send<UserModel>();
        }
        
        // Request: baseUrl/Users/GetData [GET]
        // Request action name is method name
        // Return type is "IActionResult" because the model doesn't matter
        // Anyone can access this method anonymously
        [HttpGet, AllowAnonymous]
        public static async Task<IActionResult> GetData()
        {
            return await Send<UserModel>();
        }
        
        // Request: baseUrl/Users/GetData [POST]
        // Request action name is method name without "Async"
        // Return type is "IActionResult" because the model doesn't matter
        // Anyone can access this method anonymously
        [HttpPost, AllowAnonymous]
        public static async Task<IActionResult> GetDataAsync()
        {
            return await Send<UserModel>();
        }
        
        // Request: baseUrl/Users/SomeAction [POST]
        // Request action name is assigned in the HttpPost Attribute
        // Return type is "IActionResult" because the model doesn't matter
        // Only authorized members can call this method and the authentication tokens are automatically provided for the request
        // Authentication scheme is "bearer {token}"
        // Authentication can only be used if its provided by a custom JWT middleware
        [HttpPost("SomeAction"), Authorize]
        public static async Task<IActionResult> SomeAction()
        {
            return await Send<UserModel>();
        }
    }
}