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
using System.Linq;
using DbLocalizationProvider.AdminUI.AspNetCore.Models;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    [AuthorizeRoles]
    public class ServiceController : Controller
    {
        private readonly UiConfigurationContext _config;

        public ServiceController(UiConfigurationContext config)
        {
            _config = config;
        }

        [HttpGet]
        public JsonResult Get()
        {
            return Json(PrepareViewModel());
        }

        [HttpPost]
        public JsonResult Save([FromBody] CreateOrUpdateTranslationRequestModel model)
        {
            var cmd = new CreateOrUpdateTranslation.Command(model.Key, new CultureInfo(model.Language), model.Translation);
            cmd.Execute();

            return ServiceOperationResult.Ok;
        }

        [HttpPost]
        public JsonResult Remove([FromBody] RemoveTranslationRequestModel model)
        {
            var cmd = new RemoveTranslation.Command(model.Key, new CultureInfo(model.Language));
            cmd.Execute();

            return ServiceOperationResult.Ok;
        }

        private LocalizationResourceApiModel PrepareViewModel()
        {
            var availableLanguagesQuery = new AvailableLanguages.Query { IncludeInvariant = true };
            var languages = availableLanguagesQuery.Execute();

            var getResourcesQuery = new GetAllResources.Query(true);
            var resources = getResourcesQuery.Execute().OrderBy(r => r.ResourceKey).ToList();

            var user = Request.HttpContext.User;
            var isAdmin = false;

            if(user != null)
                isAdmin = user.Identity.IsAuthenticated && _config.AuthorizedAdminRoles.Any(r => user.IsInRole(r));

            var result = new LocalizationResourceApiModel(resources, languages, _config.MaxResourceKeyPopupTitleLength, _config.MaxResourceKeyDisplayLength)
                         {
                             Options =
                             {
                                 AdminMode = isAdmin,
                                 ShowInvariantCulture = _config.ShowInvariantCulture,
                                 ShowHiddenResources = _config.ShowHiddenResources
                             }
                         };

            return result;
        }
    }
}
