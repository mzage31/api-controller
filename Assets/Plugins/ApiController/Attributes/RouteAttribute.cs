using System;

namespace ApiController.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RouteAttribute : Attribute
    {
        public string Route { get; private set; }
        public RouteAttribute(string route)
        {
            Route = route;
        }
    }
}