// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    public abstract class BaseApiModel
    {
        public BaseApiModel(IEnumerable<CultureInfo> languages)
        {
            Options = new UiOptions();
            Languages = languages.Select(l => new CultureApiModel(l.Name, l.EnglishName));
        }

        public List<JObject> Resources { get; protected set; }

        public IEnumerable<CultureApiModel> Languages { get; protected set; }

        public UiOptions Options { get; protected set; }
    }
}
