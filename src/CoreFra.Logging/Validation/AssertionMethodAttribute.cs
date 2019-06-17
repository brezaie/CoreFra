using System;

namespace CoreFra.Logging
{
    /// <summary>
    /// Indicates that the marked method is assertion method, i.e. it halts control flow if one of the conditions is satisfied.
    ///             To set the condition, mark one of the parameters with <see cref="T:NewsAnalysis.Common.Validation"/> attribute
    /// 
    /// </summary>
    /// <seealso cref="T:TadbirPardaz.Infrastructure.Annotations.AssertionConditionAttribute"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AssertionMethodAttribute : Attribute
    {
    }
}