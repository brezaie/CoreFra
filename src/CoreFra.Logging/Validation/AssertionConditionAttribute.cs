using System;

namespace CoreFra.Logging
{
    /// <summary>
    /// Indicates the condition parameter of the assertion method.
    ///             The mandatory argument of the attribute is the assertion type.
    /// 
    /// </summary>
    [Obsolete("Use ContractAnnotationAttribute instead")]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class AssertionConditionAttribute : Attribute
    {
        /// <summary>
        /// Gets condition type
        /// 
        /// </summary>
        public AssertionConditionType ConditionType { get; private set; }

        /// <summary>
        /// Initializes new instance of AssertionConditionAttribute
        /// 
        /// </summary>
        /// <param name="conditionType">Specifies condition type</param>
        public AssertionConditionAttribute(AssertionConditionType conditionType)
        {
            ConditionType = conditionType;
        }
    }

    public enum AssertionConditionType
    {
        IS_TRUE,
        IS_FALSE,
        IS_NULL,
        IS_NOT_NULL,
    }
}