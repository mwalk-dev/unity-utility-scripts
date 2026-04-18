#nullable enable
using System;

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class AllowNullAttribute : Attribute { }
}
