using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.AspNetCore.ServiceLocator
{
    public interface IServiceProviderProxy
    {
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
        object GetService(Type type);
        IEnumerable<object> GetServices(Type type);
    }
}
