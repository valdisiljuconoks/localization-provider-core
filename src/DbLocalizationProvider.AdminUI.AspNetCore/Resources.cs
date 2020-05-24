// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    [LocalizedResource]
    [Hidden]
    public class Resources
    {
        public string Title = "Admin UI";
        public string Header = "Localized Resource Editor";
        public string Export = "Export";
        public string Import = "Import";
        public string TableView = "Table View";
        public string TreeView = "Tree View";
        public string Languages = "Languages";
        public string Settings = "Settings";
        public string ShowHiddenResources = "Show Hidden Resources";
        public string Save = "Ok, Save!";
        public string Cancel = "Cancel";
        public string Remove = "Remove";
        public string RemoveConfirmation = "Do you want to remove this translation?";
        public string SearchPlaceholder = "if it gets too noisy, type filter here...";
        public string ResourceKeyColumn = "Key";
        public string InvariantCultureColumn = "Invariant Language (Invariant Country)";
        public string HiddenColumn = "Is Hidden?";
        public string FromCodeColumn = "From code?";
        public string DeleteColumn = "Delete?";
        public string DeleteConfirmation = "Do you want to throw out this resource?";
        public string ShowOnlyEmptyResources = "Show Only Empty Resources";
        public string EmptyTranslation = "Empty";
        public string CleanCache = "Clean Cache";
        public string CleanCacheConfirmation = "Wanna start with clean cache state?";
    }
}
