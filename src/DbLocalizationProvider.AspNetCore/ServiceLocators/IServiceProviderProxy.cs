using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore.ServiceLocators
{
    // TODO: Hack! This interface should be removed after making library DI compatible.
    internal interface IServiceProviderProxy
    {
        IServiceScope CreateScope();
    }
}
