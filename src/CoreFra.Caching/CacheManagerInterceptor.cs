using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Castle.Core.Internal;
using SimpleProxy;
using SimpleProxy.Interfaces;

namespace CoreFra.Caching
{
    public class CacheManagerInterceptor : IMethodInterceptor
    {
        private readonly ICacheProvider _cacheProvider;
        public TimeSpan TimeToLive { get; set; }
        private string _cacheKey;

        public CacheManagerInterceptor(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public void BeforeInvoke(InvocationContext invocationContext)
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

        public void AfterInvoke(InvocationContext invocationContext, object methodResult)
        {
            var attribute = invocationContext.GetAttributeFromMethod<CacheManagerAttribute>();

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
                _cacheProvider.AddOrUpdate(_cacheKey, methodResult);


            // Save the result to the MemoryCache
            //this.memoryCache.Set(invocationContext.GetExecutingMethodName(), methodResult);
        }

        //public async Task Invoke(AspectContext context, AspectDelegate next)
        //{
        //    var attribute = (CacheManagerInterceptor)context.ServiceMethod.GetCustomAttributes(true)
        //        .FirstOrDefault(x => x.GetType() == typeof(CacheManagerInterceptor));
        //    if (attribute == null)
        //        await next(context);

        //    var cacheKey = context.Parameters.Length > 0
        //        ? CacheKeyGenerator.GenerateCacheKey(context.ServiceMethod, context.Parameters)
        //        : context.ServiceMethod.Name;

        //    var doesCacheExist = _cacheProvider.Get(cacheKey);
        //    if (doesCacheExist != null)
        //    {
        //        context.ReturnValue = doesCacheExist;
        //    }

        //    await next(context);

        //    if (context.ReturnValue != null)
        //    {
        //        if (attribute.TimeToLive != null && attribute.TimeToLive > TimeSpan.Zero)
        //            _cacheProvider.AddOrUpdate(cacheKey, context.ReturnValue, attribute.TimeToLive);
        //        else
        //            _cacheProvider.AddOrUpdate(cacheKey, context.ReturnValue);
        //    }
        //}


     


        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    var attribute = context.Filters.FirstOrDefault(x => x.GetType() == typeof(CacheManagerInterceptor));
        //    if (attribute == null)
        //        base.OnActionExecuting(context);

        //    var parameters = context.ActionArguments.Select(x => new KeyValuePair<Type, string>(x.GetType(), x.Key)).ToList();
        //    var key = parameters.Count != 0
        //        ? GenerateCacheKey(context.Controller, context.ActionArguments)
        //        : context.Controller.ToString();

        //    base.OnActionExecuting(context);

        //}

        //public override void OnActionExecuted(ActionExecutedContext context)
        //{
        //    base.OnActionExecuted(context);
        //}

        

        
    }
}