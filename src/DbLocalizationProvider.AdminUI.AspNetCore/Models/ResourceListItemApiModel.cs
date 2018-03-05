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

using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models {
    public class ResourceListItemApiModel
    {
        public ResourceListItemApiModel(string key, ICollection<ResourceItemApiModel> translations, bool syncedFromCode)
        {
            Key = key;
            Value = translations;
            SyncedFromCode = syncedFromCode;
            AllowDelete = !syncedFromCode;
        }

        public string Key { get; }

        public ICollection<ResourceItemApiModel> Value { get; }

        public bool SyncedFromCode { get; }

        public bool AllowDelete { get; set; }

        public string DisplayKey { get; set; }
    }
}
