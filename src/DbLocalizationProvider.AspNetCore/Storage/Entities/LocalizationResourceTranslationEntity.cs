// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.AspNetCore.Storage.Entities
{
    public class LocalizationResourceTranslationEntity
    {
        public long Id { get; set; }
        public string Language { get; set; }
        public long ResourceId { get; set; }
        public string Value { get; set; }
        public DateTime ModificationDate { get; set; }
        public LocalizationResourceEntity Resource { get; set; }
    }
}