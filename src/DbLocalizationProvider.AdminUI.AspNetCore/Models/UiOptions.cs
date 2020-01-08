// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    public class UiOptions
    {
        public bool AdminMode { get; set; }

        public bool ShowInvariantCulture { get; set; }

        public bool ShowHiddenResources { get; set; }

        public bool ShowOnlyEmptyResources { get; set; }
    }
}
