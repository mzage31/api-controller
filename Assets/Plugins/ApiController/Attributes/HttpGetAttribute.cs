using System;
using JetBrains.Annotations;

namespace ApiController.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpGetAttribute : Attribute
    {
        [CanBeNull] public string ActionName { get; }

        public HttpGetAttribute([CanBeNull] string actionName = null)
        {
            ActionName = actionName;
        }
    }
}