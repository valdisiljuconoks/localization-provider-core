// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public class DbHtmlLocalizer : HtmlLocalizer, ILocalizationServicesAccessor
    {
        public DbHtmlLocalizer(IStringLocalizer stringLocalizer, ExpressionHelper expressionHelper) : base(stringLocalizer)
        {
            ExpressionHelper = expressionHelper;
        }

        public ExpressionHelper ExpressionHelper { get; }
    }
}
