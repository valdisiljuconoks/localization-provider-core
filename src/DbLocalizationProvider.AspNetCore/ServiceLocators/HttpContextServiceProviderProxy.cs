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

        public IServiceScope CreateScope()
        {
            return _contextAccessor.HttpContext.RequestServices.CreateScope();
        }
    }
}
