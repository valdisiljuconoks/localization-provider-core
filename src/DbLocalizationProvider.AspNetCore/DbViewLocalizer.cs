// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    /// <summary>
    /// An <see cref="IViewLocalizer"/> implementation that derives the resource location from the executing view's
    /// file path.
    /// </summary>
    public class DbViewLocalizer : ViewLocalizer, ILocalizationServicesAccessor
    {
        /// <summary>
        /// Creates a new <see cref="ViewLocalizer"/>.
        /// </summary>
        /// <param name="localizerFactory">The <see cref="IHtmlLocalizerFactory"/>.</param>
        /// <param name="hostingEnvironment">The <see cref="IWebHostEnvironment"/>.</param>
        /// <param name="expressionHelper">Expression helper</param>
        public DbViewLocalizer(
            IHtmlLocalizerFactory localizerFactory,
            IWebHostEnvironment hostingEnvironment,
            ExpressionHelper expressionHelper)
            : base(localizerFactory, hostingEnvironment)
        {
            ExpressionHelper = expressionHelper;
        }

        /// <summary>
        /// Expression helper
        /// </summary>
        public ExpressionHelper ExpressionHelper { get; }
    }
}
