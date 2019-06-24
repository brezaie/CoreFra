using System;
using System.Collections.Generic;
using CoreFra.Logging;
using SimpleProxy;
using SimpleProxy.Interfaces;

namespace CoreFra.Caching
{
    public class CacheInterceptor : IMethodInterceptor
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ICustomLogger _logger;
        public TimeSpan TimeToLive { get; set; }
        private string _cacheKey;

        public CacheInterceptor(ICacheProvider cacheProvider, ICustomLogger logger)
        {
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        public void BeforeInvoke(InvocationContext invocationContext)
        {
            try
            {
                var argsDictionary = new Dictionary<string, object>();
                var args = invocationContext.GetExecutingMethodInfo().GetParameters();
                for (var i = 0; i < args.Length; i++)
                {
                    var argumentValue = invocationContext.GetParameterValue(i);
                    var argumentName = args[i].Name;
                    argsDictionary.Add(argumentName, argumentValue);
                }

                _cacheKey = invocationContext.GetExecutingMethodInfo().GetParameters().Length > 0
                    ? CacheKeyGenerator.GenerateCacheKey(invocationContext.GetExecutingMethodInfo(), argsDictionary)
                    : invocationContext.GetExecutingMethodInfo().Name;


                var doesCacheExist = _cacheProvider.Get(_cacheKey);

                if (doesCacheExist != null)
                {
                    invocationContext.OverrideMethodReturnValue(doesCacheExist);
                    invocationContext.BypassInvocation();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }

        public void AfterInvoke(InvocationContext invocationContext, object methodResult)
        {
            try
            {
                var attribute = invocationContext.GetAttributeFromMethod<CacheAttribute>();

                var argsDictionary = new Dictionary<string, object>();
                var args = invocationContext.GetExecutingMethodInfo().GetParameters();
                for (var i = 0; i < args.Length; i++)
                {
                    var argumentValue = invocationContext.GetParameterValue(i);
                    var argumentName = args[i].Name;
                    argsDictionary.Add(argumentName, argumentValue);
                }

                var cacheKey = invocationContext.GetExecutingMethodInfo().GetParameters().Length > 0
                    ? CacheKeyGenerator.GenerateCacheKey(invocationContext.GetExecutingMethodInfo(), argsDictionary)
                    : invocationContext.GetExecutingMethodInfo().Name;

                var cachingDuration = new TimeSpan(attribute.Day, attribute.Hour, attribute.Minute, attribute.Second);

                if (cachingDuration > TimeSpan.Zero)
                    _cacheProvider.AddOrUpdate(cacheKey, methodResult, cachingDuration);
                else
                    _cacheProvider.AddOrUpdate(_cacheKey, methodResult, new TimeSpan(0, 10, 0));
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }
    }
}