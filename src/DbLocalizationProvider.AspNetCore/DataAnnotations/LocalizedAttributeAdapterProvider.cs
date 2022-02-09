using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations
{
    /// <inheritdoc />
    public class LocalizedAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly ResourceKeyBuilder _keyBuilder;
        private readonly IValidationAttributeAdapterProvider _base = new ValidationAttributeAdapterProvider();

        /// <summary>
        /// Creates new instance of <c>LocalizedAttributeAdapterProvider</c>.
        /// </summary>
        /// <param name="keyBuilder">Resource key builder (will be required to properly form resource key).</param>
        public LocalizedAttributeAdapterProvider(ResourceKeyBuilder keyBuilder)
        {
            _keyBuilder = keyBuilder;
        }

        /// <inheritdoc />
        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            return attribute switch
            {
                RemoteAttribute remoteAttribute => throw new NotImplementedException(),
                PageRemoteAttribute pageRemoteAttribute => throw new NotImplementedException(),
                RemoteAttributeBase remoteAttributeBase => throw new NotImplementedException(),
                CompareAttribute compareAttribute => throw new NotImplementedException(),
                CreditCardAttribute creditCardAttribute => throw new NotImplementedException(),
                CustomValidationAttribute customValidationAttribute => throw new NotImplementedException(),
                EmailAddressAttribute emailAddressAttribute => throw new NotImplementedException(),
                EnumDataTypeAttribute enumDataTypeAttribute => throw new NotImplementedException(),
                FileExtensionsAttribute fileExtensionsAttribute => throw new NotImplementedException(),
                PhoneAttribute phoneAttribute => throw new NotImplementedException(),
                UrlAttribute urlAttribute => throw new NotImplementedException(),
                DataTypeAttribute dataTypeAttribute => throw new NotImplementedException(),
                MaxLengthAttribute maxLengthAttribute => throw new NotImplementedException(),
                MinLengthAttribute minLengthAttribute => throw new NotImplementedException(),
                RangeAttribute rangeAttribute => throw new NotImplementedException(),
                RegularExpressionAttribute regularExpressionAttribute => throw new NotImplementedException(),
                RequiredAttribute requiredAttribute => new LocalizedRequiredAttributeAdapter(requiredAttribute, stringLocalizer, _keyBuilder),
                StringLengthAttribute stringLengthAttribute => new LocalizedStringLengthAttributeAdapter(stringLengthAttribute, stringLocalizer, _keyBuilder),
                _ => _base.GetAttributeAdapter(attribute, stringLocalizer)
            };
        }
    }
}
