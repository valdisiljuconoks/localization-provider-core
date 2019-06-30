// Copyright (c) 2019 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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
            var resources = new GetAllResources.Query().Execute();
            var result = exporter.Export(resources.ToList(), null);

            return new FileContentResult(Encoding.UTF8.GetBytes(result.SerializedData), result.FileMimeType) { FileDownloadName = result.FileName };
        }
    }
}
