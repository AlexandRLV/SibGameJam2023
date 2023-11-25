using System;
using JetBrains.Annotations;

namespace Common.DI
{
    [AttributeUsage(AttributeTargets.Field)]
    [MeansImplicitUse]
    public class InjectAttribute : Attribute
    {
        
    }
}