using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    [Authorize]
    public class ResourceListController : Controller
    {
        public ActionResult Index()
        {
            var raw = Request.GetEncodedUrl();
            if(!raw.EndsWith("/"))
                return Redirect(Request.PathBase.ToString() + "/");

            var provider = new EmbeddedFileProvider(typeof(ResourceListController).Assembly);
            var info = provider.GetFileInfo("adminui.html");

            return new FileStreamResult(info.CreateReadStream(), "text/html");
        }
    }
}
