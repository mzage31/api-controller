using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ApiController.Attributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace ApiController
{
    public abstract class Controller
    {
        private static AuthenticationInfo _authentication;
        protected static AuthenticationInfo Authentication => _authentication ??= new AuthenticationInfo();
        private static Stack<IAuthenticator> Authenticators { get; set; }
        private static Stack<IRequestMiddleWare> MiddleWares { get; set; }
        protected static string RouteBaseUrl { get; set; }

        public Controller()
        {
            Authenticators = new Stack<IAuthenticator>();
            MiddleWares = new Stack<IRequestMiddleWare>();
        }

        public static void Initialize(string baseUrl)
        {
            RouteBaseUrl = baseUrl;
        }


        protected static void AddAuthentication<TAuthenticator>() where TAuthenticator : class, IAuthenticator, new()
        {
            Authenticators.Push(new TAuthenticator());
        }

        protected static void AddMiddleWare<TMiddleWare>() where TMiddleWare : class, IRequestMiddleWare, new()
        {
            MiddleWares.Push(new TMiddleWare());
        }

        protected static void RunMiddleWares<TResponse>(ActionResult<TResponse> result, ActionInfo actionInfo) where TResponse : class
        {
            foreach (var authenticator in Authenticators)
                authenticator.Authenticate(Authentication, result);
            foreach (var middleWare in MiddleWares)
                middleWare.Invoke(actionInfo.Address, result);
        }
    }

    public abstract class Controller<T> : Controller where T : Controller<T>, new()
    {
        private static T _instance;
        private static T Instance => _instance ??= new T();

        private readonly Dictionary<string, MethodInfo> _methodInfos;

        public Controller()
        {
            _methodInfos = new Dictionary<string, MethodInfo>();
        }

        protected static Task<ActionResult<TResponse>> Send<TResponse>([CallerMemberName] string actionName = null) where TResponse : class
        {
            return Send<TResponse>(Array.Empty<object>(), actionName);
        }

        protected static Task<ActionResult<TResponse>> Send<TResponse>(object command, [CallerMemberName] string actionName = null) where TResponse : class
        {
            return Send<TResponse>(new[] { command }, actionName);
        }

        protected static async Task<ActionResult<TResponse>> Send<TResponse>(object[] args, [CallerMemberName] string actionName = null) where TResponse : class
        {
            var actionInfo = Instance.GetActionInfo(actionName);
            ActionResult<TResponse> result;
            if (actionInfo.IsPost)
            {
                if (args.Length == 1)
                    result = await Instance.Post<TResponse>(actionInfo, args[0], actionInfo.ActionName);
                else
                    throw new Exception("Api method marked as HttpPost must only have one method parameter. That parameter is going to be serialized and used as the request body.");
            }
            else if (actionInfo.IsGet)
                result = await Instance.Get<TResponse>(actionInfo, args, actionInfo.ActionName);
            else
                throw new Exception("Api method must have either HttpPost or HttpGet attribute.");

            RunMiddleWares(result, actionInfo);
            return result;
        }

        private async Task<ActionResult<TResponse>> Post<TResponse>(ActionInfo actionInfo, object command, string actionName = null) where TResponse : class
        {
            using var request = UnityWebRequest.Post(actionInfo.Address, "");
            var requestJson = JsonConvert.SerializeObject(command);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestJson));
            request.SetRequestHeader("content-Type", "application/json");

            if (actionInfo.Authorize)
            {
                if (Authentication.AccessToken != null)
                {
                    request.SetRequestHeader("Authorization", $"Bearer {Authentication.AccessToken}");
                }
                else
                    Debug.LogError($"{actionInfo.CallingType.Name}.{actionName} called with Authorize attribute but no AccessToken was provided.");
            }

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            var statusCode = (HttpStatusCode)(int)request.responseCode;
            if (HasError(request) || request.responseCode == 500)
            {
                var prettyCommand = JsonConvert.SerializeObject(command, Formatting.Indented);
                Debug.LogError($"{request.error} in {actionInfo.Address}:\nRequestBody: [\n{prettyCommand}\n]\nResponseBody: [\n{request.downloadHandler.text}\n]");
                return new ActionResult<TResponse>(statusCode, null);
            }

            return new ActionResult<TResponse>(statusCode, JsonConvert.DeserializeObject<TResponse>(request.downloadHandler.text));
        }

        private async Task<ActionResult<TResponse>> Get<TResponse>(ActionInfo actionInfo, object[] args, string actionName = null) where TResponse : class
        {
            var parameterNames = actionInfo.MethodInfo.GetParameters().Select(p => p.Name).ToArray();
            if (parameterNames.Length != args.Length)
                throw new Exception("All parameters must be set in the \"Get()\" method");
            var parameters = new Dictionary<string, string>();
            for (var i = 0; i < args.Length; i++)
                parameters.Add(parameterNames[i], args[i].ToString());

            var paramString = "?" + string.Join("&", parameters.Select(param => $"{param.Key}={param.Value}"));
            using var request = UnityWebRequest.Get(actionInfo.Address + paramString);

            if (actionInfo.Authorize)
            {
                if (Authentication.AccessToken != null)
                {
                    request.SetRequestHeader("Authorization", $"Bearer {Authentication.AccessToken}");
                }
                else
                    Debug.LogError($"{actionInfo.CallingType.Name}.{actionName} called with Authorize attribute but no AccessToken was provided.");
            }

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            var statusCode = (HttpStatusCode)(int)request.responseCode;
            if (HasError(request) || request.responseCode == 500)
            {
                var prettyCommand = string.Join("\n", parameters.Select(pair => $"{pair.Key} : {pair.Value}"));
                Debug.LogError($"{request.error} in {actionInfo.Address}:\nRequestBody: [\n{prettyCommand}\n]\nResponseBody: [\n{request.downloadHandler.text}\n]");
                return new ActionResult<TResponse>(statusCode, null);
            }

            var text = request.downloadHandler.text;
            TResponse resp;
            if (typeof(TResponse) == typeof(string))
                resp = request.downloadHandler.text as TResponse;
            else
                resp = JsonConvert.DeserializeObject<TResponse>(text);

            return new ActionResult<TResponse>(statusCode, resp);
        }


        private ActionInfo GetActionInfo(string actionName)
        {
            var controllerType = GetType();
            var controllerName = controllerType.Name;
            if (controllerName.EndsWith("Controller"))
                controllerName = controllerName[..^10];

            if (string.IsNullOrEmpty(controllerName))
                throw new Exception($"Controller name {controllerType.Name} is invalid");
            if (string.IsNullOrEmpty(actionName))
                throw new Exception($"Action name {actionName} is invalid");


            MethodInfo methodInfo;
            if (_methodInfos.ContainsKey(actionName))
                methodInfo = _methodInfos[actionName];
            else
            {
                methodInfo = controllerType.GetMethod(actionName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                _methodInfos.Add(actionName, methodInfo);
            }
            var authorize = methodInfo != null && methodInfo.IsDefined(typeof(AuthorizeAttribute));

            var isPost = false;
            var isGet = false;
            var overrideName = actionName;
            if (methodInfo != null)
            {
                var httpPost = methodInfo.GetCustomAttribute<HttpPostAttribute>();
                var httpGet = methodInfo.GetCustomAttribute<HttpGetAttribute>();
                if (httpPost != null)
                {
                    isPost = true;
                    if (!string.IsNullOrEmpty(httpPost.ActionName))
                        overrideName = httpPost.ActionName;
                }

                if (httpGet != null)
                {
                    isGet = true;
                    if (!string.IsNullOrEmpty(httpGet.ActionName))
                        overrideName = httpGet.ActionName;
                }
            }

            var actionRoute = overrideName;
            if (actionRoute.EndsWith("Async"))
                actionRoute = actionRoute[..^5];

            string address;
            if (controllerType.IsDefined(typeof(RouteAttribute), true))
            {
                address = RouteBaseUrl + controllerType.GetCustomAttribute<RouteAttribute>().Route;
                address = address.Replace("[controller]", controllerName);
                address = address.Replace("[action]", actionRoute);
            }
            else
                address = $"{RouteBaseUrl}{controllerName}/{actionRoute}";


            return new ActionInfo
            {
                CallingType = controllerType,
                Address = address,
                Authorize = authorize,
                MethodInfo = methodInfo,
                IsPost = isPost,
                IsGet = isGet,
                ActionName = overrideName
            };
        }

        private static bool HasError(UnityWebRequest unityWebRequest)
        {
            switch (unityWebRequest.result)
            {
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    return true;
                case UnityWebRequest.Result.InProgress:
                case UnityWebRequest.Result.Success:
                default:
                    return false;
            }
        }
    }
}