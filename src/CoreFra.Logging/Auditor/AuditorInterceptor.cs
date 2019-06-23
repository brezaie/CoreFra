using System;
using System.Collections.Generic;
using CoreFra.Domain;
using Newtonsoft.Json;
using SimpleProxy;
using SimpleProxy.Interfaces;

namespace CoreFra.Logging
{
    public class AuditorInterceptor : IMethodInterceptor
    {
        private DateTime _startTime;
        private readonly IAuditorProvider _auditorProvider;
        private readonly ICustomLogger _logger;
        public AuditorInterceptor(IAuditorProvider auditorProvider, ICustomLogger logger)
        {
            _auditorProvider = auditorProvider;
            _logger = logger;
        }

        public void BeforeInvoke(InvocationContext invocationContext)
        {
            _startTime = DateTime.Now;
        }

        public void AfterInvoke(InvocationContext invocationContext, object methodResult)
        {
            try
            {
                var endDate = DateTime.Now;
                var executionTime = (int)((endDate - _startTime).TotalMilliseconds);

                var argsDictionary = new Dictionary<string, object>();
                var args = invocationContext.GetExecutingMethodInfo().GetParameters();
                for (var i = 0; i < args.Length; i++)
                {
                    var argumentValue = invocationContext.GetParameterValue(i);
                    var argumentName = args[i].Name;
                    argsDictionary.Add(argumentName, argumentValue);
                }

                var methodInfo = invocationContext.GetExecutingMethodInfo();

                var auditEntity = new Audit
                {
                    Id = Guid.NewGuid(),
                    Input = JsonConvert.SerializeObject(argsDictionary),
                    OutPut = JsonConvert.SerializeObject(methodResult),
                    ClassName = methodInfo.Module.ToString(),
                    MethodName = methodInfo.Name,
                    StartTime = _startTime,
                    EndTime = endDate,
                    ExecutionTime = executionTime
                };

                _auditorProvider.Save(auditEntity);
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }
    }
}