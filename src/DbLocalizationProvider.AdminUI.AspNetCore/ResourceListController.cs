using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    [Authorize]
    public class ResourceListController : Controller
    {
        public ActionResult Index()
        {
            var provider = new EmbeddedFileProvider(typeof(ResourceListController).Assembly);
            var info = provider.GetFileInfo("adminui.html");

            return new FileStreamResult(info.CreateReadStream(), "text/html");
        }
    }
}
