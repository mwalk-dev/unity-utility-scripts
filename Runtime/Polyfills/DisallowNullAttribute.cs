#nullable enable
using System;

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class DisallowNullAttribute : Attribute { }
}
