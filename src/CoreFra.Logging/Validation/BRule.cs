using System.Linq;

namespace CoreFra.Logging
{
    public static class BRule
    {
        //private static ILog _logger;
        private const string DefaultErrorMessage = "business logic condition not satisfied";

        [AssertionMethod]
        public static void Assert([AssertionCondition(AssertionConditionType.IS_TRUE)]bool condition, string message, int code = -1, params object[] parameters)
        {
            Assert(condition, string.Empty, message, code, parameters.ToArray());
        }

        [AssertionMethod]
        public static void Assert([AssertionCondition(AssertionConditionType.IS_TRUE)]bool condition, string memberName, string message, int code = -1, params object[] parameters)
        {
            var exception = DoAssertion(condition, memberName, message, code, parameters.ToArray());

            if (exception == null)
                return;

            //Logger.Error(message);
            throw exception;
        }

        private static BusinessRuleException DoAssertion([AssertionCondition(AssertionConditionType.IS_TRUE)]bool condition, string memberName, string message, int code, params object[] parameters)
        {
            if (condition) return null;

            message = message == string.Empty ? DefaultErrorMessage : message;

            return new BusinessRuleException(memberName, message) { Code = code, Parameters = parameters.ToArray() };
        }
    }
}