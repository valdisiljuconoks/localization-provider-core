using DbLocalizationProvider.AspNetCore.ServiceLocators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore.EntityFramework.Extensions
{
    internal static class ServiceProviderProxyExtensions
    {
        internal static IServiceScope CreateScopedContext(this IServiceProviderProxy proxy, out DbContext context)
        {
            var scope = proxy.CreateScope();
            context = scope.ServiceProvider.GetService(Settings.ContextType) as DbContext;
            return scope;
        }
    }
}
