// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations
{
    public class LocalizedClientModelValidator : IClientModelValidatorProvider
    {
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public LocalizedClientModelValidator(IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider;
        }

        public void CreateValidators(ClientValidatorProviderContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            var type = context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType;
            var isReusable = ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders;
            var flag = false;

            foreach (var result in context.Results)
            {
                if(result.Validator != null)
                {
                    flag |= result.Validator is RequiredAttributeAdapter;
                }
                else
                {
                    if(result.ValidatorMetadata is ValidationAttribute validatorMetadata)
                    {
                        flag |= validatorMetadata is RequiredAttribute;
                        var attributeAdapter = _validationAttributeAdapterProvider.GetAttributeAdapter(validatorMetadata,
                                                                                                       new ValidationStringLocalizer(type,
                                                                                                                                     context.ModelMetadata.PropertyName,
                                                                                                                                     validatorMetadata));
                        if(attributeAdapter != null)
                        {
                            result.Validator = attributeAdapter;
                            result.IsReusable = isReusable;
                        }
                    }
                }
            }

            if(flag || !context.ModelMetadata.IsRequired)
                return;

            context.Results.Add(new ClientValidatorItem
                                {
                                    Validator = _validationAttributeAdapterProvider.GetAttributeAdapter(new RequiredAttribute(),
                                                                                                        new ValidationStringLocalizer(type,
                                                                                                                                      context.ModelMetadata.PropertyName,
                                                                                                                                      new RequiredAttribute())),
                                    IsReusable = isReusable
            });
        }
    }
}
