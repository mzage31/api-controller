using System;
using JetBrains.Annotations;

namespace ApiController.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpPostAttribute : Attribute
    {
        [CanBeNull] public string ActionName { get; }

        public HttpPostAttribute([CanBeNull] string actionName = null)
        {
            ActionName = actionName;
        }
    }
}