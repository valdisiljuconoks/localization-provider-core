// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using System.Linq.Expressions;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore.Extensions
{
    public static class IHtmlLocalizerExtensions
    {
        public static LocalizedHtmlString GetString(this IHtmlLocalizer target, Expression<Func<object>> model, params object[] formatArguments)
        {
            return target[ExpressionHelper.GetFullMemberName(model), formatArguments];
        }

        public static LocalizedHtmlString GetStringByCulture(this IHtmlLocalizer target, Expression<Func<object>> model, CultureInfo language, params object[] formatArguments)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if(language == null)
                throw new ArgumentNullException(nameof(language));

            return target.WithCulture(language)[ExpressionHelper.GetFullMemberName(model), formatArguments];
        }

        public static LocalizedHtmlString GetString<T>(this IHtmlLocalizer<T> target, Expression<Func<T, object>> model, params object[] formatArguments)
        {
            return target[ExpressionHelper.GetFullMemberName(model), formatArguments];
        }

        public static LocalizedHtmlString GetStringByCulture<T>(
            this IHtmlLocalizer<T> target,
            Expression<Func<T, object>> model,
            CultureInfo language,
            params object[] formatArguments)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if(language == null)
                throw new ArgumentNullException(nameof(language));

            return target.WithCulture(language)[ExpressionHelper.GetFullMemberName(model), formatArguments];
        }
    }
}
