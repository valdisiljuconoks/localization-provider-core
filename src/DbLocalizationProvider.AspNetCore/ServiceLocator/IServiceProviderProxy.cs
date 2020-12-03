using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.AspNetCore.ServiceLocator
{
    // TODO: Hack! This interface should be removed after making library DI compatible.
    internal interface IServiceProviderProxy
    {
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
        object GetService(Type type);
        IEnumerable<object> GetServices(Type type);
    }
}
