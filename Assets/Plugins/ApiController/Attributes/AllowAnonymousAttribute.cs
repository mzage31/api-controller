using System;

namespace ApiController.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AllowAnonymousAttribute : Attribute
    {
    }
}