using System;

namespace EnsureThat
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class ContractAnnotationAttribute : Attribute
    {
    }
}
