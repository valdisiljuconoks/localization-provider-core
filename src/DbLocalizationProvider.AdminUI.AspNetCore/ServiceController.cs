// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AdminUI.AspNetCore.Models;
using DbLocalizationProvider.AdminUI.AspNetCore.Security;
using DbLocalizationProvider.AdminUI.Models;
using DbLocalizationProvider.AspNetCore.Import;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Import;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

[Route("/localization-admin/api/service", Name = "LocAdminUIRoute")]
[Authorize(Policy = AccessPolicy.Name)]
public class ServiceController : ControllerBase
{
    private readonly UiConfigurationContext _config;
    private readonly ICommandExecutor _commandExecutor;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ConfigurationContext _configurationContext;

    public ServiceController(
        UiConfigurationContext config,
        ICommandExecutor commandExecutor,
        IQueryExecutor queryExecutor,
        ConfigurationContext configurationContext
    )
    {
        _config = config;
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _queryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
        _configurationContext = configurationContext;
    }

    [HttpGet]
    [Route("get", Name = "LocAdminGet")]
    public IActionResult Get()
    {
        return new ApiResponse(PrepareViewModel());
    }

    [HttpGet]
    [Route("gettree", Name = "LocAdminGetTree")]
    public ApiResponse GetTree()
    {
        return new ApiResponse(PrepareTreeViewModel());
    }

    [HttpPost]
    [Route("save", Name = "LocAdminSave")]
    public JsonResult Save([FromBody] CreateOrUpdateTranslationRequestModel model)
    {
        var cmd = new CreateOrUpdateTranslation.Command(model.Key, new CultureInfo(model.Language), model.Translation);
        _commandExecutor.Execute(cmd);

        return ServiceOperationResult.Ok;
    }

    [HttpPost]
    [Route("add", Name = "LocAdminAdd")]
    public JsonResult Add([FromBody] AddResourceAndTranslationRequestModel model)
    {
        var resource = new LocalizationResource(model.Key, true) { IsHidden = false, IsModified = true };
        resource.Translations.Add(new LocalizationResourceTranslation { Value = model.Translation, Language = model.Language });

        var createNewResourceCmd = new CreateNewResource.Command(resource);
        _commandExecutor.Execute(createNewResourceCmd);

        return ServiceOperationResult.Ok;
    }

    [HttpPost]
    [Route("validate", Name = "LocAdminValidate")]
    public ValidationResponse ValidateFile(IFormFile importFile)
    {
        if (importFile == null)
        {
            return new ValidationResponse(null);
        }

        var streamReader = new StreamReader(importFile.OpenReadStream());
        var fileContent = streamReader.ReadToEnd();

        if (string.IsNullOrEmpty(fileContent))
        {
            return new ValidationResponse(null);
        }

        var parser = _configurationContext.Import.Providers.FindByExtension(new FileInfo(importFile.FileName).Extension);
        var result = parser.Parse(fileContent);
        var workflow = new ResourceImportWorkflow(_commandExecutor, _queryExecutor);
        var (resources, _) = GetResources();
        var detectedImportChanges = workflow.DetectChanges(result.Resources, resources);

        return new ValidationResponse(
            detectedImportChanges
                .Where(x => !x.ExistingResource.IsHidden.HasValue || !x.ExistingResource.IsHidden.Value)
                .ToArray());
    }

    [HttpPost]
    [Route("import", Name = "LocAdminImport")]
    public JsonResult ImportFile([ModelBinder(typeof(NewtonJsonModelBinder))][FromBody]List<DetectedImportChange> changes)
    {
        var detectedImportChanges = changes.Where(c => c.Selected).ToList();

        var workflow = new ResourceImportWorkflow(_commandExecutor, _queryExecutor);
        var messages = workflow.ImportChanges(detectedImportChanges).ToList();

        return new JsonResult(messages);
    }

    [HttpPost]
    [Route("remove", Name = "LocAdminRemove")]
    public JsonResult Remove([FromBody] RemoveTranslationRequestModel model)
    {
        var cmd = new RemoveTranslation.Command(model.Key, new CultureInfo(model.Language));
        _commandExecutor.Execute(cmd);

        return ServiceOperationResult.Ok;
    }

    [HttpPost]
    [Route("delete", Name = "LocAdminDelete")]
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
        var (resources, languages) = GetResources();
        var visibleLanguages = GetVisibleLanguages(languages);

        var result = new LocalizationResourceApiModel(
            resources,
            languages,
            visibleLanguages ?? languages,
            _config.MaxResourceKeyPopupTitleLength,
            _config.MaxResourceKeyDisplayLength,
            BuildOptions());

        return result;
    }

    private IEnumerable<AvailableLanguage> GetVisibleLanguages(IEnumerable<AvailableLanguage> availableLanguages)
    {
        var cookie = Request.Cookies["LocalizationProvider_VisibleLanguages"];
        if (string.IsNullOrEmpty(cookie))
        {
            return null;
        }

        var languagesInCookie = cookie
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !l.Equals("invariant", StringComparison.InvariantCultureIgnoreCase))
            .Select(l => new CultureInfo(l));

        return availableLanguages.Where(a => languagesInCookie.Contains(a.CultureInfo));
    }

    private LocalizationResourceApiTreeModel PrepareTreeViewModel()
    {
        var (resources, languages) = GetResources();
        var visibleLanguages = GetVisibleLanguages(languages);

        var result = new LocalizationResourceApiTreeModel(resources,
                                                          languages,
                                                          visibleLanguages ?? languages,
                                                          _config.MaxResourceKeyPopupTitleLength,
                                                          _config.MaxResourceKeyDisplayLength,
                                                          BuildOptions());

        return result;
    }

    private (List<LocalizationResource>, IEnumerable<AvailableLanguage>) GetResources()
    {
        var availableLanguagesQuery = new AvailableLanguages.Query { IncludeInvariant = false };
        var languages = _queryExecutor.Execute(availableLanguagesQuery);

        var getResourcesQuery = new GetAllResources.Query(true);
        var resources = _queryExecutor
            .Execute(getResourcesQuery)
            .OrderBy(_ => _.ResourceKey)
            .ToList();

        return (resources, languages);
    }

    private UiOptions BuildOptions()
    {
        return new UiOptions
        {
            AdminMode = true,
            ShowInvariantCulture = _config.ShowInvariantCulture,
            ShowHiddenResources = _config.ShowHiddenResources,
            ShowDeleteButton = !_config.HideDeleteButton
        };
    }
}
