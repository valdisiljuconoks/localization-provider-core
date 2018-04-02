using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute, IFilterFactory
    {
        public AuthorizeRolesAttribute() : this(null)
        {
            // this constructor is needed to apply attribute to controller or method without supplying arguments
            // instance will be still created via `IFilterFactory` interface - going through service provider
        }

        public AuthorizeRolesAttribute(UiConfigurationContext config)
        {
            if(config == null)
            {
                config = UiConfigurationContext.Current;
            }

            Roles = string.Join(",", config.AuthorizedAdminRoles.Concat(config.AuthorizedEditorRoles).Distinct());
            IsReusable = false;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<AuthorizeRolesAttribute>();
        }

        public bool IsReusable { get; }
    }
}
