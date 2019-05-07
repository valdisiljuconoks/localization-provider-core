using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Core.AspNetSample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Core.AspNetSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly LocalizationProvider _provider;

        public HomeController(LocalizationProvider provider, IOptions<MvcOptions> options)
        {
            _provider = provider;

            var asms = GetAssemblies().Where(a => a.FullName.Contains("DbLocalizationProvider"));
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach(var reference in asm.GetReferencedAssemblies())
                    if(!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }
            }
            while(stack.Count > 0);
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

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                                    new CookieOptions
                                    {
                                        Expires = DateTimeOffset.UtcNow.AddYears(1)
                                    }
                                   );

            return LocalRedirect(returnUrl);
        }
    }
}
