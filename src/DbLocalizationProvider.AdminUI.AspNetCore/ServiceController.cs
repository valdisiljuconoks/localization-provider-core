// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AdminUI.AspNetCore.Models;
using DbLocalizationProvider.AdminUI.AspNetCore.Security;
using DbLocalizationProvider.AdminUI.Models;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    [Authorize(Policy = AccessPolicy.Name)]
    public class ServiceController : ControllerBase
    {
        private readonly UiConfigurationContext _config;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;

        public ServiceController(UiConfigurationContext config, ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            _config = config;
            _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
            _queryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new ApiResponse(PrepareViewModel());
        }

        [HttpGet]
        public ApiResponse GetTree()
        {
            return new ApiResponse(PrepareTreeViewModel());
        }

        private LocalizationResourceApiTreeModel PrepareTreeViewModel()
        {
            var (resources, languages, isAdmin) = GetResources();
            var result = new LocalizationResourceApiTreeModel(resources,
                languages,
                _config.MaxResourceKeyPopupTitleLength,
                _config.MaxResourceKeyDisplayLength,
                BuildOptions(isAdmin));

            return result;
        }

        [HttpPost]
        public JsonResult Save([FromBody] CreateOrUpdateTranslationRequestModel model)
        {
            var cmd = new CreateOrUpdateTranslation.Command(model.Key, new CultureInfo(model.Language), model.Translation);
            _commandExecutor.Execute(cmd);

            return ServiceOperationResult.Ok;
        }

        [HttpPost]
        public JsonResult Remove([FromBody] RemoveTranslationRequestModel model)
        {
            var cmd = new RemoveTranslation.Command(model.Key, new CultureInfo(model.Language));
            _commandExecutor.Execute(cmd);

            return ServiceOperationResult.Ok;
        }

        [HttpPost]
        public JsonResult Delete([FromBody] DeleteResourceRequestModel model)
        {
            if (!_config.HideDeleteButton)
            {
                var cmd = new DeleteResource.Command(model.Key);
                _commandExecutor.Execute(cmd);
            }

            return ServiceOperationResult.Ok;
        }

        private LocalizationResourceApiModel PrepareViewModel()
        {
            var (resources, languages, isAdmin) = GetResources();
            var result = new LocalizationResourceApiModel(
                resources,
                languages,
                _config.MaxResourceKeyPopupTitleLength,
                _config.MaxResourceKeyDisplayLength,
                BuildOptions(isAdmin));

            return result;
        }

        private (List<LocalizationResource>, IEnumerable<CultureInfo>, bool) GetResources()
        {
            var availableLanguagesQuery = new AvailableLanguages.Query { IncludeInvariant = false };
            var languages = _queryExecutor.Execute(availableLanguagesQuery);

            var getResourcesQuery = new GetAllResources.Query(true);
            var resources = _queryExecutor
                .Execute(getResourcesQuery)
                .OrderBy(_ => _.ResourceKey)
                .ToList();

            var user = Request.HttpContext.User;
            var isAdmin = false;

            if(user != null)
                isAdmin = user.Identity.IsAuthenticated && _config.AuthorizedAdminRoles.Any(_ => user.IsInRole(_));

            return (resources, languages, isAdmin);
        }

        private UiOptions BuildOptions(bool isAdmin)
        {
            return new UiOptions
            {
                AdminMode = isAdmin,
                ShowInvariantCulture = _config.ShowInvariantCulture,
                ShowHiddenResources = _config.ShowHiddenResources,
                ShowDeleteButton = !_config.HideDeleteButton
            };
        }
    }
}
