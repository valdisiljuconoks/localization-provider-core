// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Areas._4D5A2189D188417485BF6C70546D34A1.Pages;

public class AdminUITreeViewModel : BasePage
{
    public AdminUITreeViewModel(
        IOptions<ConfigurationContext> configurationContext,
        UiConfigurationContext uiContext,
        IQueryExecutor queryExecutor,
        ICommandExecutor commandExecutor)
        : base(configurationContext, uiContext, queryExecutor, commandExecutor) { }
}
