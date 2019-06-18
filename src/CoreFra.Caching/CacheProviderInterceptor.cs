using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreFra.Caching
{
    public class CacheProviderInterceptor : AbstractInterceptor
    {
        private readonly ICacheProvider _cacheProvider;

        public CacheProviderInterceptor(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var attribute = (CacheManagerAttribute)context.ServiceMethod.GetCustomAttributes(true)
                .FirstOrDefault(x => x.GetType() == typeof(CacheManagerAttribute));
            if (attribute == null)
                await next(context);

            var cacheKey = context.Parameters.Length > 0
                ? CacheKeyGenerator.GenerateCacheKey(context.ServiceMethod, context.Parameters)
                : context.ServiceMethod.Name;

            var doesCacheExist = _cacheProvider.Get(cacheKey);
            if (doesCacheExist != null)
            {
                context.ReturnValue = doesCacheExist;
            }

            await next(context);

            if (context.ReturnValue != null)
            {
                if (attribute.TimeToLive != null && attribute.TimeToLive > TimeSpan.Zero)
                    _cacheProvider.AddOrUpdate(cacheKey, context.ReturnValue, attribute.TimeToLive);
                else
                    _cacheProvider.AddOrUpdate(cacheKey, context.ReturnValue);
            }
        }


     


        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    var attribute = context.Filters.FirstOrDefault(x => x.GetType() == typeof(CacheProviderInterceptor));
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