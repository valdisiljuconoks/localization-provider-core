// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public class DbViewLocalizer : ViewLocalizer, ILocalizationServicesAccessor
    {
        public DbViewLocalizer(
            IHtmlLocalizerFactory localizerFactory,
            IWebHostEnvironment hostingEnvironment,
            ExpressionHelper expressionHelper)
            : base(localizerFactory, hostingEnvironment)
        {
            ExpressionHelper = expressionHelper;
        }

        public ExpressionHelper ExpressionHelper { get; }
    }
}
