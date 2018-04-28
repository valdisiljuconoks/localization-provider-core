using System.Diagnostics;
using DbLocalizationProvider.Core.AspNetSample.Models;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.Core.AspNetSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly LocalizationProvider _provider;

        public HomeController(LocalizationProvider provider)
        {
            _provider = provider;
        }

        public IActionResult Index()
        {
            ViewData["TestString"] = _provider.GetString(() => Resources.Shared.CommonResources.Yes);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
