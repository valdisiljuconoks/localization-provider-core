// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    public class AuthorizeRolesAttribute : TypeFilterAttribute
    {
        public AuthorizeRolesAttribute()
            : base(typeof(RoleRequirementFilter))
        {
            var config = UiConfigurationContext.Current;

            var roles = config.AuthorizedAdminRoles.Distinct().ToArray();

            Arguments = roles;
        }
    }

    public class RoleRequirementFilter : IAuthorizationFilter
    {
        readonly IEnumerable<string> _roles;

        public RoleRequirementFilter()
        {
            var config = UiConfigurationContext.Current;

            var roles = config.AuthorizedAdminRoles.Distinct().ToArray();

            this._roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasRole = false;

            foreach (var role in this._roles)
            {
                if (context.HttpContext.User.IsInRole(role))
                {
                    hasRole = true;
                }
            }

            if (!hasRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
