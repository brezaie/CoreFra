using System;
using SimpleProxy;
using SimpleProxy.Interfaces;

namespace CoreFra.Logging
{
    public class AuditorInterceptor : IMethodInterceptor
    {
        private DateTime _startTime;
        public void BeforeInvoke(InvocationContext invocationContext)
        {
            _startTime = DateTime.Now;
        }

        public void AfterInvoke(InvocationContext invocationContext, object methodResult)
        {
            throw new System.NotImplementedException();
        }
    }
}