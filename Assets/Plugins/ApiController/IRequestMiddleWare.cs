namespace ApiController
{
    public interface IRequestMiddleWare
    {
        void Invoke<T>(string address, ActionResult<T> result) where T : class;
    }
}