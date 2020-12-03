using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.AspNetCore.Storage.Locators
{
    public interface IServiceProviderProxy
    {
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
        object GetService(Type type);
        IEnumerable<object> GetServices(Type type);
    }
}
