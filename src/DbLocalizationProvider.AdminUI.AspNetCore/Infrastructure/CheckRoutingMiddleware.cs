using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure
{
    public class CheckRoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private static ConcurrentDictionary<string, object> _middlewareNames = new ConcurrentDictionary<string, object>();

        public CheckRoutingMiddleware(RequestDelegate next)
        {
            _next = next;
            var middlewareName = next.Target.GetType().FullName;
            _middlewareNames.TryAdd(middlewareName, null);

            if (middlewareName == "DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure.AdminUIMarkerMiddleware")
            {
                // AdminUi has been added - let's check if we have routing in place already
                if (!_middlewareNames.ContainsKey("Microsoft.AspNetCore.Builder.RouterMiddleware")
                    && !_middlewareNames.ContainsKey("Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware"))
                {
                    throw new InvalidOperationException(
                        "Routing has not been initialized. Invoke 'UseDbLocalizationProviderAdminUI' after routing system setup.");
                }
            }
        }

        public Task Invoke(HttpContext context)
        {
            return _next(context);
        }
    }
}
