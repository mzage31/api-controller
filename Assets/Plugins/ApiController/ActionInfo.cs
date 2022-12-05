using System;
using System.Reflection;

namespace ApiController
{
    public sealed class ActionInfo
    {
        public Type CallingType { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public string Address { get; set; }
        public bool Authorize { get; set; }
        public bool IsPost { get; set; }
        public bool IsGet { get; set; }
        public string ActionName { get; set; }
    }
}