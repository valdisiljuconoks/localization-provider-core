using System.IO;
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

        public FileResult ExportResources(string format = "json")
        {
            var exporter = ConfigurationContext.Current.Export.Providers.FindById(format);
            var resources = new GetAllResources.Query().Execute();
            var result = exporter.Export(resources.ToList(), null);

            using(var stream = new MemoryStream())
                using(var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(result.SerializedData);
                    writer.Flush();
                    stream.Position = 0;

                    return File(stream, result.FileMimeType, result.FileName);
                }
        }
    }
}
