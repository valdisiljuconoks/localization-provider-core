using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Core.AspNetSample.Resources
{
    [LocalizedResource]
    public class SampleResources
    {
        public static string PageHeader => "This is page header";

        public string PageHeader2 { get; set; } = "This is page header 2";

        [WeirdCustom("Weird html attribute value")]
        public static string SomeHtmlResource => "<span><b>THIS IS BOLD</b></span>";

        [WeirdCustom("Weird enum attribute value")]
        public static SomeEnum SomeEnumProperty => SomeEnum.ValueOne;

        public static string ThisIsPrettyLongResourceKeyNameJustToTestHowBadItLooksInAdministrativeUserInterfaceResourceListPageTable => "This is pretty long";
    }

    [LocalizedResource]
    public enum SomeEnum
    {
        None = 0,

        [Display(Name = "Value [1]")]
        ValueOne = 1,

        [Display(Name = "Value [2]")]
        ValueTwo = 2,

        [Display(Name = "Value [3]")]
        ValueThree = 3
    }
}
