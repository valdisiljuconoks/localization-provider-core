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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppContext = DbLocalizationProvider.AspNetCore.ClientsideProvider.AppContext;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IHtmlHelperOfTExtensions
    {
        public static IHtmlContent Translate(
            this IHtmlHelper htmlHelper,
            Expression<Func<object>> expression,
            params object[] formatArguments)
        {
            return TranslateByCulture(htmlHelper, expression, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static IHtmlContent Translate(
            this IHtmlHelper htmlHelper,
            Enum target,
            params object[] formatArguments)
        {
            return TranslateByCulture(htmlHelper, target, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static IHtmlContent Translate(
            this IHtmlHelper htmlHelper,
            Expression<Func<object>> expression,
            Type customAttribute,
            params object[] formatArguments)
        {
            return TranslateByCulture(htmlHelper, expression, customAttribute, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static IHtmlContent TranslateByCulture(
            this IHtmlHelper htmlHelper,
            Enum target,
            CultureInfo language,
            params object[] formatArguments)
        {
            return new HtmlString(target.TranslateByCulture(language, formatArguments));
        }

        public static IHtmlContent TranslateByCulture(
            this IHtmlHelper htmlHelper,
            Expression<Func<object>> expression,
            Type customAttribute,
            CultureInfo language,
            params object[] formatArguments)
        {
            if(htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));
            if(expression == null)
                throw new ArgumentNullException(nameof(expression));
            if(!typeof(Attribute).IsAssignableFrom(customAttribute))
                throw new ArgumentException($"Given type `{customAttribute.FullName}` is not of type `System.Attribute`");
            if(language == null)
                throw new ArgumentNullException(nameof(language));

            var resourceKey = ResourceKeyBuilder.BuildResourceKey(ExpressionHelper.GetFullMemberName(expression), customAttribute);

            return new HtmlString(LocalizationProvider.Current.GetStringByCulture(resourceKey, language, formatArguments));
        }

        public static IHtmlContent TranslateByCulture(
            this IHtmlHelper htmlHelper,
            Expression<Func<object>> expression,
            CultureInfo language,
            params object[] formatArguments)
        {
            if(htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));
            if(expression == null)
                throw new ArgumentNullException(nameof(expression));
            if(language == null)
                throw new ArgumentNullException(nameof(language));

            return new HtmlString(LocalizationProvider.Current.GetStringByCulture(expression, language, formatArguments));
        }

        public static IHtmlContent TranslateFor<TModel, TResult>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TResult>> expression,
            params object[] formatArguments)
        {
            return TranslateForByCulture(htmlHelper, expression, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static IHtmlContent TranslateFor<TModel, TResult>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TResult>> expression,
            Type customAttribute,
            params object[] formatArguments)
        {
            return TranslateForByCulture(htmlHelper, expression, customAttribute, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static IHtmlContent TranslateForByCulture<TModel, TResult>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TResult>> expression,
            CultureInfo language,
            params object[] formatArguments)
        {
            if(htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));
            if(expression == null)
                throw new ArgumentNullException(nameof(expression));
            if(language == null)
                throw new ArgumentNullException(nameof(language));

            return new HtmlString(LocalizationProvider.Current.GetStringByCulture(ExpressionHelper.GetFullMemberName(expression), language, formatArguments));
        }

        public static IHtmlContent TranslateForByCulture<TModel, TResult>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TResult>> expression,
            Type customAttribute,
            CultureInfo language,
            params object[] formatArguments)
        {
            if(htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));
            if(expression == null)
                throw new ArgumentNullException(nameof(expression));
            if(!typeof(Attribute).IsAssignableFrom(customAttribute))
                throw new ArgumentException($"Given type `{customAttribute.FullName}` is not of type `System.Attribute`");
            if(language == null)
                throw new ArgumentNullException(nameof(language));

            var resourceKey = ResourceKeyBuilder.BuildResourceKey(ExpressionHelper.GetFullMemberName(expression), customAttribute);

            return new HtmlString(LocalizationProvider.Current.GetStringByCulture(resourceKey, language, formatArguments));
        }

        public static IHtmlContent DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, params object[] formatArguments)
        {
            return DescriptionByCultureFor(htmlHelper, expression, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static IHtmlContent DescriptionByCultureFor<TModel, TValue>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression,
            CultureInfo language,
            params object[] formatArguments)
        {
            if(htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));
            if(expression == null)
                throw new ArgumentNullException(nameof(expression));
            if(language == null)
                throw new ArgumentNullException(nameof(language));

            return new HtmlString(LocalizationProvider.Current.GetStringByCulture(ExpressionHelper.GetFullMemberName(expression) + "-Description", language, formatArguments));
        }

        public static IHtmlContent GetTranslations<TModel>(
            this IHtmlHelper<TModel> helper,
            Type containerType,
            string language = null,
            string alias = null,
            bool debug = false,
            bool camelCase = false)
        {
            return GetTranslations((IHtmlHelper)helper, containerType, language, alias, debug, camelCase);
        }

        public static IHtmlContent GetTranslations<TModel>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<object>> model,
            string language = null,
            string alias = null,
            bool debug = false,
            bool camelCase = false)
        {
            return GenerateScriptTag(language, alias, debug, ExpressionHelper.GetFullMemberName(model), camelCase);
        }

        public static IHtmlContent GetTranslations(
            this IHtmlHelper helper,
            Type containerType,
            string language = null,
            string alias = null,
            bool debug = false,
            bool camelCase = false)
        {
            if(containerType == null)
                throw new ArgumentNullException(nameof(containerType));

            return GenerateScriptTag(language, alias, debug, ResourceKeyBuilder.BuildResourceKey(containerType), camelCase);
        }

        public static IHtmlContent GetTranslations(
            this IHtmlHelper helper,
            Expression<Func<object>> model,
            string language = null,
            string alias = null,
            bool debug = false,
            bool camelCase = false)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            return GenerateScriptTag(language, alias, debug, ExpressionHelper.GetFullMemberName(model), camelCase);
        }

        private static IHtmlContent GenerateScriptTag(string language, string alias, bool debug, string resourceKey, bool camelCase)
        {
            // if 1st request
            var mergeScript = string.Empty;
            var httpItems = AppContext.Service.HttpContext.Items;

            if(httpItems["__DbLocalizationProvider_JsHandler_1stRequest"] == null)
            {
                httpItems.Add("__DbLocalizationProvider_JsHandler_1stRequest", false);
                mergeScript = $"<script src=\"{ClientsideConfigurationContext.RootPath}/{ClientsideConfigurationContext.DeepMergeScriptName}\"></script>";
            }

            var url = $"{ClientsideConfigurationContext.RootPath}/{resourceKey.Replace("+", "---")}";
            var parameters = new Dictionary<string, string>();

            if(!string.IsNullOrEmpty(language))
                parameters.Add("lang", language);

            if(!string.IsNullOrEmpty(alias))
                parameters.Add("alias", alias);

            if(debug)
                parameters.Add("debug", "true");

            if(camelCase)
                parameters.Add("camel", "true");

            if(parameters.Any())
                url += "?" + ToQueryString(parameters);

            return new HtmlString($"{mergeScript}<script src=\"{url}\"></script>");
        }

        private static string ToQueryString(Dictionary<string, string> parameters)
        {
            if(parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if(!parameters.Any())
                return string.Empty;

            return string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }
}
