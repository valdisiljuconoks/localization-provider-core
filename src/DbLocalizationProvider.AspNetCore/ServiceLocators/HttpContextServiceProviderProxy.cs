using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore.ServiceLocators
{

    // TODO: Hack! This class should be removed after making library DI compatible.
    internal class HttpContextServiceProviderProxy : IServiceProviderProxy
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpContextServiceProviderProxy(IHttpContextAccessor contextAccessor)
        {
            this._contextAccessor = contextAccessor;
        }

        public T GetService<T>()
        {
            return _contextAccessor.HttpContext.RequestServices.GetService<T>();
        }

        public IEnumerable<T> GetServices<T>()
        {
            return _contextAccessor.HttpContext.RequestServices.GetServices<T>();
        }

        public object GetService(Type type)
        {
            return _contextAccessor.HttpContext.RequestServices.GetService(type);
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return _contextAccessor.HttpContext.RequestServices.GetServices(type);
        }
    }
}
