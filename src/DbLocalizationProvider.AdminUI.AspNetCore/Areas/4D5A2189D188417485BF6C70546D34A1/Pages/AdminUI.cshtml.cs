using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Areas._4D5A2189D188417485BF6C70546D34A1.Pages
{
    public class AdminUIViewModel : PageModel
    {
        public IActionResult OnGet()
        {
            // this is need in order for the Vue and other "sub-components" to load correctly.
            // otherwise - resources will be mapped to one level above and will fail to load
            if(!Request.GetEncodedUrl().EndsWith('/'))
                return Redirect(Request.GetEncodedUrl() + "/");

            return Page();
        }
    }
}
