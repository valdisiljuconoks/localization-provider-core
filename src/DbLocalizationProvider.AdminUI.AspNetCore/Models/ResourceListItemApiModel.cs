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
using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    public class ResourceListItemApiModel
    {
        public ResourceListItemApiModel(string key, ICollection<ResourceItemApiModel> translations, bool syncedFromCode)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = translations ?? throw new ArgumentNullException(nameof(translations));
            SyncedFromCode = syncedFromCode;
            AllowDelete = !syncedFromCode;

            var displayLength = 120;
            var titleLength = 80;

            DisplayKey = $"{key.Substring(0, key.Length > displayLength ? displayLength : key.Length)}{(key.Length > displayLength ? "..." : "")}";
            TitleKey = $"{(key.Length > titleLength ? "..." : "")}{key.Substring(key.Length - Math.Min(titleLength, key.Length))}";
        }

        public string Key { get; }

        public ICollection<ResourceItemApiModel> Value { get; }

        public bool SyncedFromCode { get; }

        public bool AllowDelete { get; }

        public string DisplayKey { get; }

        public string TitleKey { get; }
    }
}
