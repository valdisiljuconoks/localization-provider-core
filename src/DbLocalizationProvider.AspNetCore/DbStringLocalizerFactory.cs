// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Internal;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    /// <summary>
    /// A factory that creates <see cref="IStringLocalizerFactory" /> instances.
    /// </summary>
    public class DbStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ExpressionHelper _expressionHelper;
        private readonly CultureInfo _language;
        private readonly ILocalizationProvider _localizationProvider;

        /// <summary>
        /// Creates new instance of this class
        /// </summary>
        /// <param name="localizationProvider">Localization provider</param>
        /// <param name="expressionHelper">Expression builder</param>
        public DbStringLocalizerFactory(ILocalizationProvider localizationProvider, ExpressionHelper expressionHelper)
        {
            _localizationProvider = localizationProvider;
            _expressionHelper = expressionHelper;
        }

        private DbStringLocalizerFactory(
            CultureInfo language,
            ILocalizationProvider localizationProvider,
            ExpressionHelper expressionHelper) : this(localizationProvider, expressionHelper)
        {
            _language = language;
        }

        /// <summary>
        /// Creates an <see cref="IStringLocalizer"/> using the <see cref="System.Reflection.Assembly"/> and
        /// <see cref="Type.FullName"/> of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="resourceSource">The <see cref="Type"/>.</param>
        /// <returns>The <see cref="IStringLocalizer"/>.</returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            return new DbStringLocalizer(_language, _localizationProvider, _expressionHelper);
        }

        /// <summary>
        /// Creates an <see cref="IStringLocalizer"/>.
        /// </summary>
        /// <param name="baseName">The base name of the resource to load strings from.</param>
        /// <param name="location">The location to load resources from.</param>
        /// <returns>The <see cref="IStringLocalizer"/>.</returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return new DbStringLocalizer(_language, _localizationProvider, _expressionHelper);
        }

        /// <summary>
        /// Changes language of given IStringLocalizer factory
        /// </summary>
        /// <param name="language">Language to change to</param>
        /// <returns>The <see cref="DbStringLocalizerFactory" />.</returns>
        public DbStringLocalizerFactory ChangeLanguage(CultureInfo language)
        {
            return new DbStringLocalizerFactory(language, _localizationProvider, _expressionHelper);
        }
    }
}
