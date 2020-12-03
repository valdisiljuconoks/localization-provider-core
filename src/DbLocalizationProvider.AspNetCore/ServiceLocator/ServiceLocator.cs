using System;

namespace DbLocalizationProvider.AspNetCore.ServiceLocator
{
    internal class ServiceLocator
    {
        private static IServiceProviderProxy _proxy;

        public static IServiceProviderProxy ServiceProvider =>
            _proxy ?? throw new Exception("You should Initialize the ServiceProvider before using it.");

        public static void Initialize(IServiceProviderProxy proxy)
        {
            _proxy = proxy;
        }
    }
}
