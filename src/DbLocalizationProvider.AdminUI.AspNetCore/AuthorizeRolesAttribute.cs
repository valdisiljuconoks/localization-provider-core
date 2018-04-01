using Microsoft.AspNetCore.Authorization;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute()
        {
            Roles = string.Join(",", UiConfigurationContext.Current.AuthorizedAdminRoles);
        }
    }
}
