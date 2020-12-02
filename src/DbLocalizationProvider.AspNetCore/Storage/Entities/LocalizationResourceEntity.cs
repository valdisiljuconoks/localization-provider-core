// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbLocalizationProvider.AspNetCore.Storage.Entities
{
    public class LocalizationResourceEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Author { get; set; }
        public bool FromCode { get; set; }
        public bool IsHidden { get; set; }
        public bool IsModified { get; set; }
        public DateTime ModificationDate { get; set; }
        public string ResourceKey { get; set; }
        public string Notes { get; set; }
        public ICollection<LocalizationResourceTranslationEntity> Translations { get; set; }
    }
}
