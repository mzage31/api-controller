namespace ApiController
{
    public interface IAuthenticator
    {
        void Authenticate<T>(AuthenticationInfo info, ActionResult<T> result) where T : class;
    }
}