#nullable enable
using System;

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class MemberNotNullAttribute : Attribute
    {
        public MemberNotNullAttribute(params string[] members) { }
    }
}
