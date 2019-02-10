using System;
using System.Globalization;
using System.Linq.Expressions;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    }
}
