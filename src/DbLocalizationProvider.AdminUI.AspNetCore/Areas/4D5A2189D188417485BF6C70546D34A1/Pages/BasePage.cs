// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Linq;
using System.Text;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Areas._4D5A2189D188417485BF6C70546D34A1.Pages
{
    public class BasePage : PageModel
    {
        private const string _lastViewCookieName = "LocalizationProvider_LastView";

        public IActionResult OnGet()
        {
            // this is need in order for the Vue and other "sub-components" to load correctly.
            // otherwise - resources will be mapped to one level above and will fail to be fetched
            var url = Request.GetEncodedUrl();

            if(!url.EndsWith('/'))
                return Redirect(url + "/");

            var lastView = Request.Cookies[_lastViewCookieName];
            var defaultView = UiConfigurationContext.Current.DefaultView;
            var isTreeView = url.Contains("tree");

            if(!string.IsNullOrEmpty(lastView))
                return Page();

            // set view from config
            Response.Cookies.Append(_lastViewCookieName, UiConfigurationContext.Current.DefaultView.ToString(), new CookieOptions { HttpOnly = true });
            if(!isTreeView && defaultView == ResourceListView.Tree)
                Response.Redirect(url + "tree/");

            return Page();
        }

        public FileResult OnGetExport(string format = "json")
        {
            var exporter = ConfigurationContext.Current.Export.Providers.FindById(format);
            var resources = new GetAllResources.Query(true).Execute();
            var result = exporter.Export(resources.ToList(), null);

            return new FileContentResult(Encoding.UTF8.GetBytes(result.SerializedData), result.FileMimeType) { FileDownloadName = result.FileName };
        }
    }
}
