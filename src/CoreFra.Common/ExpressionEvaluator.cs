using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreFra.Common
{
    public static class ExpressionEvaluator
    {
        public static T Evaluate<T>(string expression, IEnumerable<KeyValuePair<Type, string>> parameters, object[] values)
        {
            var expressions = parameters.Select(x => Expression.Parameter(x.Key, x.Value));
            LambdaExpression lambda = DynamicExpression.ParseLambda(expressions.ToArray(), typeof(T), expression);
            var compiled = lambda.Compile();
            return (T)compiled.DynamicInvoke(values);
        }
    }
}