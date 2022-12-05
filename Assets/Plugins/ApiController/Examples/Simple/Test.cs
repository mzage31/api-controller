using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace ApiController.Examples
{
    public class Test : MonoBehaviour
    {
        private async void Start()
        {
            await GetUser1();
            await GetUser2();
        }

        private static async Task GetUser1()
        {
            var result = await UsersController.GetUser1();
            if (result is { StatusCode: HttpStatusCode.OK })
                Debug.Log($"{result.StatusCode:G}");
            else
                Debug.LogError($"{result.StatusCode:G}");
        }

        private static async Task GetUser2()
        {
            var result = await UsersController.GetUser2();
            if (result is { StatusCode: HttpStatusCode.OK, Response: { Id: 2 } user })
                Debug.Log($"User {user.Id}'s data:\n{user.Name}\n{user.Username}\n{user.Email}");
            else
                Debug.LogError($"{result.StatusCode:G} - {result.Response?.Id ?? -1}");
        }
    }
}