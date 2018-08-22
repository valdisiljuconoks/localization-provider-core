// Copyright (c) 2018 Valdis Iljuconoks.
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
                            result.IsReusable = true;
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
                                    IsReusable = true
                                });
        }
    }
}
