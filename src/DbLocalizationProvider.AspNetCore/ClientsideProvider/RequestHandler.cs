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

using System.Globalization;
using System.Threading.Tasks;
using DbLocalizationProvider.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonConverter = DbLocalizationProvider.Json.JsonConverter;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider
{
    public class RequestHandler
    {
        // must have , otherwise exception at runtime
        public RequestHandler(RequestDelegate next) { }

        public async Task Invoke(HttpContext context)
        {
            var response = GenerateResponse(context);

            context.Response.ContentType = "application/javascript";
            await context.Response.WriteAsync(response);
        }

        private string GenerateResponse(HttpContext context)
        {
            var languageName = !context.Request.Query.ContainsKey("lang")
                                   ? CultureInfo.CurrentUICulture.Name
                                   : context.Request.Query["lang"].ToString();

            var filename = ExtractFileName(context);

            if(filename == ClientsideConfigurationContext.DeepMergeScriptName)
                return
                    "/* https://github.com/KyleAMathews/deepmerge */ var jsResourceHandler=function(){function r(r){return!(c=r,!c||'object'!=typeof c||(t=r,n=Object.prototype.toString.call(t),'[object RegExp]'===n||'[object Date]'===n||(o=t,o.$$typeof===e)));var t,n,o,c}var e='function'==typeof Symbol&&Symbol.for?Symbol.for('react.element'):60103;function t(e,t){var n;return(!t||!1!==t.clone)&&r(e)?o((n=e,Array.isArray(n)?[]:{}),e,t):e}function n(r,e,n){return r.concat(e).map(function(r){return t(r,n)})}function o(e,c,a){var u,f,y,i,b=Array.isArray(c);return b===Array.isArray(e)?b?((a||{arrayMerge:n}).arrayMerge||n)(e,c,a):(f=c,y=a,i={},r(u=e)&&Object.keys(u).forEach(function(r){i[r]=t(u[r],y)}),Object.keys(f).forEach(function(e){r(f[e])&&u[e]?i[e]=o(u[e],f[e],y):i[e]=t(f[e],y)}),i):t(c,a)}return{deepmerge:function(r,e,t){return o(r,e,t)}}}();";

            var debugMode = context.Request.Query.ContainsKey("debug");
            var camelCase = context.Request.Query.ContainsKey("camel");
            var alias = !context.Request.Query.ContainsKey("alias") ? "jsl10n" : context.Request.Query["alias"].ToString();

            var cacheKey = CacheHelper.GenerateKey(filename, languageName, debugMode, alias);
            var cache = context.RequestServices.GetService<ICacheManager>();

            if(!(cache.Get(cacheKey) is string responseObject))
            {
                responseObject = GetJson(filename, languageName, debugMode, camelCase);
                responseObject = $"window.{alias} = jsResourceHandler.deepmerge(window.{alias} || {{}}, {responseObject})";

                cache.Insert(cacheKey, responseObject);
            }

            return responseObject;
        }

        private string GetJson(string filename, string languageName, bool debugMode, bool camelCase)
        {
            var settings = new JsonSerializerSettings();
            var _converter = new JsonConverter();

            if(camelCase)
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            if(debugMode)
                settings.Formatting = Formatting.Indented;

            return JsonConvert.SerializeObject(_converter.GetJson(filename, languageName, camelCase), settings);
        }

        private static string ExtractFileName(HttpContext context)
        {
            var result = context.Request.Path.ToString().Replace(ClientsideConfigurationContext.RootPath, string.Empty);
            result = result.StartsWith("/") ? result.TrimStart('/') : result;
            result = result.EndsWith("/") ? result.TrimEnd('/') : result;

            return result.Replace("---", "+");
        }
    }
}
