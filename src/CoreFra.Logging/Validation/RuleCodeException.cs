using System.ComponentModel;

namespace CoreFra.Logging
{
    public enum RuleCodeException
    {
        [Description("عملیات با موفقیت انجام شد")]
        OperationSucceeded,
        [Description("عملیات با شکست مواجه شد")]
        OperationFailed
    }
}