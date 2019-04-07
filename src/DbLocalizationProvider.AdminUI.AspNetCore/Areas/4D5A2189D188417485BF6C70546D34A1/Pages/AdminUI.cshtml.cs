using System.Linq;
using System.Text;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Queries;
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

        public FileResult OnGetExport(string format = "json")
        {
            var exporter = ConfigurationContext.Current.Export.Providers.FindById(format);
            var resources = new GetAllResources.Query().Execute();
            var result = exporter.Export(resources.ToList(), null);

            return new FileContentResult(Encoding.UTF8.GetBytes(result.SerializedData), result.FileMimeType) { FileDownloadName = result.FileName };
        }
    }
}
