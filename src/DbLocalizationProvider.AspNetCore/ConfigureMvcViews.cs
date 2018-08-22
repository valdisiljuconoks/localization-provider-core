using DbLocalizationProvider.AspNetCore.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore
{
    public class ConfigureMvcViews : IConfigureOptions<MvcViewOptions>
    {
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public ConfigureMvcViews(IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider;
        }

        public void Configure(MvcViewOptions options)
        {
            options.ClientModelValidatorProviders.Insert(0, new LocalizedClientModelValidator(_validationAttributeAdapterProvider));
        }
    }
}
