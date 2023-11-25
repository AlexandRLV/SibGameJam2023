using System;
using JetBrains.Annotations;

namespace Common.DI
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    [MeansImplicitUse]
    public class ConstructAttribute : Attribute
    {
    }
}