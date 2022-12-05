# ASP.NET Core 6 looking API Controllers in unity!

```cs
[ApiController, Route("[controller]/[action]")]
public class AccountController : Controller<AccountController>
{
    public AccountController()
    {
        AddAuthentication<JwtAuthenticator>();
        AddMiddleWare<LoggerMiddleWare>();
    }

    [HttpPost, AllowAnonymous]
    public static Task<ActionResult<Response<RequestCodeResponse>>> RequestCode(RequestCodeCommand command)
    {
        return Send<Response<RequestCodeResponse>>(command);
    }

    [HttpPost, AllowAnonymous]
    public static async Task<ActionResult<Response<LoginResponse>>> LoginAsync(LoginCommand command)
    {
        return await Send<Response<LoginResponse>>(command);
    }

    [HttpPost("RefreshToken"), AllowAnonymous]
    public static Task<ActionResult<Response<RefreshTokenResponse>>> RefreshTokenWithWrongName(RefreshTokenCommand command)
    {
        return Send<Response<RefreshTokenResponse>>(command);
    }

    [HttpPost, Authorize]
    public static Task<ActionResult<Response>> SetDisplayName(SetDisplayNameCommand command)
    {
        return Send<Response>(command);
    }
    
    [HttpGet, Authorize]
    public static Task<ActionResult<Response<string>>> GetData(string param1, int param2)
    {
        return Send<Response<string>>(new object[] { param1, param2 });
    }
}
```

## Request Middlewares

```cs
public class LoggerMiddleWare : IRequestMiddleWare
{
    public void Invoke<T>(string address, ActionResult<T> result) where T : class
    {
        if (result.Handled)
            return;
        var json = JsonConvert.SerializeObject(result, Formatting.Indented);
        Debug.Log($"LoggerMiddleWare ({address}):\n{json}");
    }
}
```

## Handling Authentication
```cs
public class JwtAuthenticator : IAuthenticator
{
    public void Authenticate<T>(AuthenticationInfo info, ActionResult<T> result) where T : class
    {
        if (result.StatusCode == HttpStatusCode.OK)
        {
            switch (result.Response)
            {
                case Response<LoginResponse> { Code: ApiResultCode.Ok, Model: { } } r1:
                    Debug.Log($"Authenticating: {r1.Model.RefreshToken}");
                    info.AccessToken = r1.Model.AccessToken;
                    info.RefreshToken = r1.Model.RefreshToken;
                    break;
                case Response<RefreshTokenResponse> { Code: ApiResultCode.Ok, Model: { } } r2:
                    Debug.Log($"Authenticating: {r2.Model.RefreshToken}");
                    info.AccessToken = r2.Model.AccessToken;
                    info.RefreshToken = r2.Model.RefreshToken;
                    break;
            }
        }
    }
}
```
