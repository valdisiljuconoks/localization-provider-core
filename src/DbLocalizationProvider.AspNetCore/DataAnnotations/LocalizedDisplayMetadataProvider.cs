// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations
{
    public class LocalizedDisplayMetadataProvider : IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            var theAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;
            var containerType = context.Key.ContainerType;

            if(containerType == null) return;
            if(containerType.GetCustomAttribute<LocalizedModelAttribute>() == null) return;

            var currentMetaData = modelMetadata.DisplayName?.Invoke();

            modelMetadata.DisplayName = () => ConfigurationContext.Current.ResourceLookupFilter(currentMetaData)
                ? ModelMetadataLocalizationHelper.GetTranslation(currentMetaData)
                : ModelMetadataLocalizationHelper.GetTranslation(containerType, propertyName);

            var displayAttribute = theAttributes.OfType<DisplayAttribute>().FirstOrDefault();
            if(displayAttribute?.Description != null)
            {
                modelMetadata.Description = () =>
                    ModelMetadataLocalizationHelper.GetTranslation(containerType, $"{propertyName}-Description");
            }
        }
    }
}
