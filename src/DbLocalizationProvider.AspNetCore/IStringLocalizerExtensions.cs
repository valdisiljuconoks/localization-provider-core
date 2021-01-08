// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IStringLocalizerExtensions
    {
        public static LocalizedString GetString(
            this IStringLocalizer target,
            Expression<Func<object>> model,
            params object[] formatArguments)
        {
            return target[GetMemberName(target, model), formatArguments];
        }

        public static LocalizedString GetStringByCulture(
            this IStringLocalizer target,
            Expression<Func<object>> model,
            CultureInfo language,
            params object[] formatArguments)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            if (target is ICultureAwareStringLocalizer cultureAwareLocalizer)
            {
                return cultureAwareLocalizer.ChangeLanguage(language)[GetMemberName(target, model), formatArguments];
            }

            return null;
        }

        public static LocalizedString GetString<T>(
            this IStringLocalizer<T> target,
            Expression<Func<T, object>> model,
            params object[] formatArguments)
        {
            return target[GetMemberName(target, model), formatArguments];
        }

        public static LocalizedString GetStringByCulture<T>(
            this IStringLocalizer<T> target,
            Expression<Func<T, object>> model,
            CultureInfo language,
            params object[] formatArguments)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            if (target is ICultureAwareStringLocalizer cultureAwareLocalizer)
            {
                return cultureAwareLocalizer.ChangeLanguage(language)[GetMemberName(target, model), formatArguments];
            }

            return null;
        }

        private static string GetMemberName(IStringLocalizer target, LambdaExpression model)
        {
            if (target is ILocalizationServicesAccessor accessor)
            {
                return accessor.ExpressionHelper.GetFullMemberName(model);
            }

            return string.Empty;
        }
    }
}
