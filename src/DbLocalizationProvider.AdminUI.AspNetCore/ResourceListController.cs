using System.IO;
using System.Linq;
using System.Text;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    [AuthorizeRoles]
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

        public FileResult ExportResources(string format = "json")
        {
            var exporter = ConfigurationContext.Current.Export.Providers.FindById(format);
            var resources = new GetAllResources.Query().Execute();
            var result = exporter.Export(resources.ToList(), null);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(result.SerializedData);
            writer.Flush();
            stream.Position = 0;

            return File(stream, result.FileMimeType, result.FileName);
        }
    }
}
